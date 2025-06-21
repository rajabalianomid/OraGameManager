using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Data.Migrations;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure.Proxy;
using Ora.GameManaging.Server.Infrastructure.Services;
using Ora.GameManaging.Server.Models;
using Ora.GameManaging.Server.Models.Adapter;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;

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
            public List<object> TargetPlayers { get; set; } = [];
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
        public void StartGroupTurnRotating(string roomId, List<object> userOrGroups, int durationSeconds)
        {
            Cancel(roomId);

            var state = new GroupTimerState
            {
                TokenSource = new CancellationTokenSource(),
                IsPaused = false,
                RemainingSeconds = durationSeconds,
                TargetPlayers = userOrGroups
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
                // Each item in TargetPlayers can be string (single) or List<string> (group)
                for (int idx = 0; idx < state.TargetPlayers.Count; idx++)
                {
                    List<string> userIds = state.TargetPlayers[idx] switch
                    {
                        string single => [single],
                        List<string> group => group,
                        _ => []
                    };

                    int seconds = state.RemainingSeconds;

                    if (GameManager.Rooms.TryGetValue(roomId, out var room))
                    {
                        for (int i = seconds; i >= 0; i--)
                        {
                            state.RemainingSeconds = i;

                            while (state.IsPaused)
                                await Task.Delay(200, state.TokenSource.Token);

                            // Send per-player info (gRPC, TurnInfo, etc.)
                            foreach (var userId in userIds)
                            {
                                if (room.Players.TryGetValue(userId, out var player))
                                {
                                    using var scope = serviceProvider.CreateScope();
                                    var gameRoomService = scope.ServiceProvider.GetRequiredService<IGameRoomService>();
                                    await gameRoomService.UpdateCurrentTurnAndSyncCacheAsync(room.AppId, roomId.Split(":").Last(), userId);

                                    room.CurrentTurnPlayerId = userId;
                                    var latestinfo = await grpcAdapter.Do<ThridPartInfo, LastInformationRequestModel>(
                                        new LastInformationRequestModel { RequestModel = room.Serialize() });

                                    var jsonObj = latestinfo?.ToJsonNode();
                                    if (jsonObj != null)
                                    {
                                        jsonObj["remindTime"] = i;
                                        await hubContext.Clients.Client(player.ConnectionId).SendAsync("TurnInfo", jsonObj);
                                    }
                                }
                            }

                            // Timeout for all in this group
                            if (i == 0)
                            {
                                var connectionIds = userIds
                                    .Select(uid => room.Players.TryGetValue(uid, out var p) ? p.ConnectionId : null)
                                    .Where(cid => cid != null)
                                    .Cast<string>()
                                    .ToList();
                                await hubContext.Clients.Clients(connectionIds).SendAsync("TurnTimeout", userIds);
                            }
                            else
                            {
                                await Task.Delay(1000, state.TokenSource.Token);
                            }
                        }
                    }

                    // Reset for the next group
                    state.RemainingSeconds = seconds;
                }

                _turnFinishedCallback?.Invoke(roomId);
                Cancel(roomId);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"[TurnManager] Error in simultaneous/group timer: {ex.Message}");
            }
        }

        // Rotating group timer: each player has their turn one after another
        private async Task RunGroupTimerRotating(string roomId, GroupTimerState state)
        {
            try
            {
                for (int idx = 0; idx < state.TargetPlayers.Count; idx++)
                {
                    // Support both single and group
                    List<string> userIds = state.TargetPlayers[idx] switch
                    {
                        string single => [single],
                        List<string> group => group,
                        _ => []
                    };

                    int seconds = state.RemainingSeconds;

                    if (GameManager.Rooms.TryGetValue(roomId, out var room))
                    {
                        // Track users to move to notInCurrentTurn
                        var usersToMove = new List<string>();

                        for (int i = seconds; i >= 0; i--)
                        {
                            state.RemainingSeconds = i;

                            while (state.IsPaused)
                                await Task.Delay(200, state.TokenSource.Token);

                            var tasks = userIds
                                .Select(async userId =>
                                {
                                    var result = await HandlePlayerTurnInfoAsync(room, state, userIds, idx, i, userId, true);
                                    if (!result.IsAlive || !result.Success)
                                    {
                                        usersToMove.Add(userId);
                                    }
                                    return result;
                                }).ToList();

                            await Task.WhenAll(tasks);

                            // Flatten state.TargetPlayers to a list of user IDs (strings)
                            var allTargetUserIds = state.TargetPlayers
                                .SelectMany(tp => tp is string s ? [s] : tp is List<string> list ? list : Enumerable.Empty<string>())
                                .ToList();
                            var notInCurrentTurn = allTargetUserIds.Except(userIds).ToList();

                            // Move users with IsAlive == false or Success == false to notInCurrentTurn for next tick
                            if (usersToMove.Count > 0)
                            {
                                userIds = [.. userIds.Except(usersToMove)];
                                notInCurrentTurn.AddRange(usersToMove);
                                usersToMove.Clear();
                            }

                            // If no users alive in this turn, break
                            if (userIds.Count == 0)
                                break;

                            var tasksnotInCurrentTurn = notInCurrentTurn
                                .Select(userId => HandlePlayerTurnInfoAsync(room, state, userIds, idx, i, userId, false))
                                .ToList();

                            await Task.WhenAll(tasksnotInCurrentTurn);


                            // Timeout for all in this turn
                            if (i == 0)
                            {
                                var connectionIds = userIds
                                    .Select(uid => room.Players.TryGetValue(uid, out var p) ? p.ConnectionId : null)
                                    .Where(cid => cid != null)
                                    .Cast<string>()
                                    .ToList();
                                await hubContext.Clients.Clients(connectionIds).SendAsync("TurnTimeout", userIds);
                            }
                            else
                            {
                                await Task.Delay(1000, state.TokenSource.Token);
                            }
                            Console.WriteLine(i + " seconds left ,phase " + room.Phase);
                        }
                    }



                    // Reset for the next player/group
                    state.RemainingSeconds = seconds;
                }

                _turnFinishedCallback?.Invoke(roomId);
                Cancel(roomId);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"[TurnManager] Error in rotating/group timer: {ex.Message}");

                if (ex is AggregateException aggEx)
                {
                    foreach (var inner in aggEx.InnerExceptions)
                        Console.WriteLine(inner);
                }
            }
        }

        // Helper to handle player turn info
        private async Task<HandlePlayerTurnInfoModel> HandlePlayerTurnInfoAsync(GameRoom room, GroupTimerState state, List<string> userIds, int idx, int i, string userId, bool isYourTurn)
        {
            var result = new HandlePlayerTurnInfoModel() { Success = true };

            if (!room.Players.TryGetValue(userId, out var player))
            {
                result.Messages.Add($"Player {userId} not found in room {room.RoomId}.");
                result.Success = false;
                return result;
            }

            var latestinfo = await grpcAdapter.Do<ThridPartInfo, LastInformationRequestModel>(
                new LastInformationRequestModel { RequestModel = room.Serialize(userId, isYourTurn) });

            if (latestinfo?.Data?.ToString() is string jsonData)
            {
                var baseUserInfo = JsonSerializer.Deserialize<BaseUserModel>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                result.IsAlive = baseUserInfo?.IsAlive ?? false;
                if (isYourTurn && !result.IsAlive)
                {
                    return result;
                }
            }

            var nextForceTurns = latestinfo?.ExtraInfo?.ForceNextTurns;

            if (isYourTurn)
            {
                using var scope = serviceProvider.CreateScope();
                var gameRoomService = scope.ServiceProvider.GetRequiredService<IGameRoomService>();
                await gameRoomService.UpdateCurrentTurnAndSyncCacheAsync(room.AppId, room.RoomId.Split(":").Last(), userId);

                room.CurrentTurnPlayerId = userId;
            }

            // Only for single turns, handle nextForceTurns
            if (userIds.Count == 1 && nextForceTurns != null && nextForceTurns.Count != 0)
            {
                nextForceTurns.Reverse();
                foreach (var f in nextForceTurns)
                    state.TargetPlayers.Insert(idx + 1, f);
                await grpcAdapter.Do(new MakeEmptyOfChallengeModel { AppId = room.AppId, RoomId = room.RoomId });
            }

            var jsonObj = new
            {
                latestinfo?.Data,
                ExtraInfo = new
                {
                    latestinfo?.ExtraInfo?.ForceNextTurns,
                    ExtraPlayerInfo = JsonNode.Parse(JsonSerializer.Serialize(player.ExtraInfos, new JsonSerializerOptions { PropertyNamingPolicy = new LowerCaseNamingPolicy(), PropertyNameCaseInsensitive = true })),
                    IsYourTurn = isYourTurn,
                    ReminderTime = i
                }
            }?.ToJsonNode();
            if (jsonObj != null)
            {
                await hubContext.Clients.Client(player.ConnectionId).SendAsync("TurnInfo", jsonObj);
                Console.WriteLine(player.ConnectionId + " Player: " + userId + "Is Turn: " + isYourTurn);
            }
            return result;
        }

        public void RegisterTurnFinishedCallback(Action<string> callback)
        {
            _turnFinishedCallback = callback;
        }
    }
}
