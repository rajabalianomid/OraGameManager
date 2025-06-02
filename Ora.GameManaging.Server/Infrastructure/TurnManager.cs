using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure.Proxy;
using Ora.GameManaging.Server.Infrastructure.Services;
using Ora.GameManaging.Server.Models.Adapter;
using System.Collections.Concurrent;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class TurnManager(IHubContext<GameHub> hubContext, IServiceProvider serviceProvider, GrpcAdapter grpcAdapter)
    {
        // Callback for notifying when a turn is finished
        private Action<string>? _turnFinishedCallback;

        // Internal state for each room's timer
        private class GroupTimerState
        {
            public CancellationTokenSource TokenSource { get; set; } = new();
            public bool IsPaused { get; set; }
            public int RemainingSeconds { get; set; }
            public List<string> TargetPlayers { get; set; } = new();
        }

        private readonly ConcurrentDictionary<string, GroupTimerState> _groupTimers = new();

        // Simultaneous group timer for all selected players
        public void StartGroupTurnSimultaneous(string roomId, List<string> userIds, int durationSeconds)
        {
            Cancel(roomId);

            var state = new GroupTimerState
            {
                TokenSource = new CancellationTokenSource(),
                IsPaused = false,
                RemainingSeconds = durationSeconds,
                TargetPlayers = [.. userIds]
            };
            _groupTimers[roomId] = state;

            _ = RunGroupTimerSimultaneous(roomId, state); // Fire & forget
        }

        // Rotating group timer, each player gets timer one after another
        public void StartGroupTurnRotating(string roomId, List<string> userIds, int durationSeconds)
        {
            Cancel(roomId);

            var state = new GroupTimerState
            {
                TokenSource = new CancellationTokenSource(),
                IsPaused = false,
                RemainingSeconds = durationSeconds,
                TargetPlayers = userIds.ToList()
            };
            _groupTimers[roomId] = state;

            _ = RunGroupTimerRotating(roomId, state); // Fire & forget
        }

        public void PauseGroupTimer(string roomId)
        {
            if (_groupTimers.TryGetValue(roomId, out var state))
                state.IsPaused = true;
        }

        public void ResumeGroupTimer(string roomId)
        {
            if (_groupTimers.TryGetValue(roomId, out var state))
                state.IsPaused = false;
        }

        public void Cancel(string roomId)
        {
            if (_groupTimers.TryRemove(roomId, out var state))
                state.TokenSource.Cancel();
        }

        // Simultaneous group timer: everyone sees the timer
        private async Task RunGroupTimerSimultaneous(string roomId, GroupTimerState state)
        {
            try
            {
                for (int i = state.RemainingSeconds; i >= 0; i--)
                {
                    state.RemainingSeconds = i;

                    while (state.IsPaused)
                    {
                        await Task.Delay(200, state.TokenSource.Token);
                    }

                    List<string> currentConnectionIds = [];
                    if (GameManager.Rooms.TryGetValue(roomId, out var room))
                    {
                        foreach (var userId in state.TargetPlayers)
                        {
                            if (room.Players.TryGetValue(userId, out var player))
                                currentConnectionIds.Add(player.ConnectionId);
                        }
                    }

                    // Broadcast timer tick ONLY to target players (not whole group)
                    await hubContext.Clients.Clients(currentConnectionIds).SendAsync("TimerTick", i);

                    if (i == 0)
                    {
                        // Timeout ONLY to target players
                        await hubContext.Clients.Clients(currentConnectionIds).SendAsync("TurnTimeout", state.TargetPlayers);
                        Cancel(roomId); // Clean up after finish
                    }
                    else
                    {
                        await Task.Delay(1000, state.TokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Timer canceled
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TurnManager] Error in simultaneous timer: {ex.Message}");
            }
        }

        // Rotating group timer: each player has their turn one after another
        private async Task RunGroupTimerRotating(string roomId, GroupTimerState state)
        {
            try
            {
                foreach (var userId in state.TargetPlayers)
                {
                    int seconds = state.RemainingSeconds;
                    string? currentConnectionId = null;
                    if (GameManager.Rooms.TryGetValue(roomId, out var room) &&
                        room.Players.TryGetValue(userId, out var player))
                    {
                        using var scope = serviceProvider.CreateScope();
                        var gameRoomService = scope.ServiceProvider.GetRequiredService<IGameRoomService>();
                        await gameRoomService.UpdateCurrentTurnAndSyncCacheAsync(room.AppId, roomId.Split(":").Last(), userId); // Ensure current turn is set
                        currentConnectionId = player.ConnectionId;
                    }

                    for (int i = seconds; i >= 0; i--)
                    {
                        state.RemainingSeconds = i;

                        while (state.IsPaused)
                        {
                            await Task.Delay(200, state.TokenSource.Token);
                        }

                        if (!string.IsNullOrEmpty(currentConnectionId))
                        {
                            // Ensure `room` is not null before using it
                            if (room != null)
                            {
                                room.CurrentTurnPlayerId = userId;
                                var latestinfo = await grpcAdapter.Do<object, LastInformationRequestModel>(new LastInformationRequestModel { RequestModel = room.Serialize() });
                                var jsonObj = latestinfo.ToJsonNode();
                                if (jsonObj != null)
                                {
                                    jsonObj["remindTime"] = i;
                                    var modifiedJson = jsonObj.ToJsonSerialize();

                                    await hubContext.Clients.Client(currentConnectionId).SendAsync("TurnInfo", jsonObj);
                                }
                                // Send timer tick only to the current player
                                //await hubContext.Clients.Client(currentConnectionId).SendAsync("TimerTick", i);
                                //await hubContext.Clients.Client(currentConnectionId).SendAsync("TurnInfo", latestinfo);
                            }
                        }

                        if (i == 0)
                        {
                            if (!string.IsNullOrEmpty(currentConnectionId))
                                await hubContext.Clients.Client(currentConnectionId).SendAsync("TurnTimeout");
                            // Next player automatically
                        }
                        else
                        {
                            await Task.Delay(1000, state.TokenSource.Token);
                        }
                    }
                    // Reset for the next player (if you want fresh timer for each player)
                    state.RemainingSeconds = seconds;
                }
                // Notify GameManager that the rotating turn is finished
                _turnFinishedCallback?.Invoke(roomId);

                Cancel(roomId); // Clean up after all players finished
            }
            catch (OperationCanceledException)
            {
                // Timer canceled
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TurnManager] Error in rotating timer: {ex.Message}");
            }
        }

        public void RegisterTurnFinishedCallback(Action<string> callback)
        {
            _turnFinishedCallback = callback;
        }
    }
}
