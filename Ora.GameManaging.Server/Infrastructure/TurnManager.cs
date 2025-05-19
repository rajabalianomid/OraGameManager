using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Infrastructure.Proxy;
using System.Collections.Concurrent;
using System.Security.AccessControl;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class TurnManager(IHubContext<GameHub> hubContext, GrpcHelloService grpcHelloService)
    {

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
        public void StartGroupTurnSimultaneous(string roomId, List<string> playerIds, int durationSeconds)
        {
            Cancel(roomId);

            var state = new GroupTimerState
            {
                TokenSource = new CancellationTokenSource(),
                IsPaused = false,
                RemainingSeconds = durationSeconds,
                TargetPlayers = playerIds.ToList()
            };
            _groupTimers[roomId] = state;

            _ = RunGroupTimerSimultaneous(roomId, state); // Fire & forget
        }

        // Rotating group timer, each player gets timer one after another
        public void StartGroupTurnRotating(string roomId, List<string> playerIds, int durationSeconds)
        {
            Cancel(roomId);

            var state = new GroupTimerState
            {
                TokenSource = new CancellationTokenSource(),
                IsPaused = false,
                RemainingSeconds = durationSeconds,
                TargetPlayers = playerIds.ToList()
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
                    var result= await grpcHelloService.SayHelloAsync("Mafia", "Test");
                    state.RemainingSeconds = i;

                    while (state.IsPaused)
                    {
                        await Task.Delay(200, state.TokenSource.Token);
                    }

                    // Broadcast timer tick ONLY to target players (not whole group)
                    await hubContext.Clients.Clients(state.TargetPlayers).SendAsync("TimerTick", i);

                    if (i == 0)
                    {
                        // Timeout ONLY to target players
                        await hubContext.Clients.Clients(state.TargetPlayers).SendAsync("TurnTimeout", state.TargetPlayers);
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
                foreach (var playerId in state.TargetPlayers)
                {
                    int seconds = state.RemainingSeconds;
                    for (int i = seconds; i >= 0; i--)
                    {
                        state.RemainingSeconds = i;

                        while (state.IsPaused)
                        {
                            await Task.Delay(200, state.TokenSource.Token);
                        }

                        // Send timer tick only to the current player
                        await hubContext.Clients.Client(playerId).SendAsync("TimerTick", i);

                        if (i == 0)
                        {
                            await hubContext.Clients.Client(playerId).SendAsync("TurnTimeout");
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
    }
}
