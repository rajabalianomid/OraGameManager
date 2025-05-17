using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class TurnManager(NotificationManager notification)
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _tokens = new();
        private readonly ConcurrentDictionary<string, List<string>> _groupTurnOrder = new();
        private readonly ConcurrentDictionary<string, int> _groupTurnIndex = new();

        // Simultaneous group turn: all selected players are active at the same time
        public void StartGroupTurnSimultaneous(string roomId, List<string> playerConnectionIds, int duration)
        {
            if (playerConnectionIds == null || playerConnectionIds.Count == 0)
                return;

            var cts = new CancellationTokenSource();
            _tokens[roomId] = cts;
            var token = cts.Token;

            _ = Task.Run(async () =>
            {
                await notification.SendTurnChangedToPlayers(playerConnectionIds, "It's your turn!");
                for (int i = duration; i >= 0; i--)
                {
                    if (token.IsCancellationRequested) return;
                    await notification.SendTimerTickToPlayers(playerConnectionIds, i);
                    await Task.Delay(1000, token);
                }
                if (!token.IsCancellationRequested)
                {
                    await notification.SendTurnTimeoutToPlayers(playerConnectionIds, "Timeout!");
                    await notification.SendGroupTurnEnded(roomId);
                }
            });
        }

        // Rotating group turn: one-by-one, single pass
        public void StartGroupTurnRotating(string roomId, List<string> playerConnectionIds, int duration)
        {
            if (playerConnectionIds == null || playerConnectionIds.Count == 0)
                return;

            _groupTurnOrder[roomId] = playerConnectionIds;
            _groupTurnIndex[roomId] = 0;
            RunNextTurn(roomId, duration);
        }

        private void RunNextTurn(string roomId, int duration)
        {
            if (!_groupTurnOrder.TryGetValue(roomId, out var order) ||
                !_groupTurnIndex.TryGetValue(roomId, out var idx) ||
                idx >= order.Count)
            {
                notification.SendGroupTurnEnded(roomId);
                _groupTurnOrder.TryRemove(roomId, out _);
                _groupTurnIndex.TryRemove(roomId, out _);
                return;
            }

            var currentPlayerId = order[idx];
            var cts = new CancellationTokenSource();
            _tokens[roomId] = cts;
            var token = cts.Token;

            _ = Task.Run(async () =>
            {
                await notification.SendTurnChangedToPlayers(new List<string> { currentPlayerId }, "It's your turn!");
                for (int i = duration; i >= 0; i--)
                {
                    if (token.IsCancellationRequested) return;
                    await notification.SendTimerTickToPlayers(new List<string> { currentPlayerId }, i);
                    await Task.Delay(1000, token);
                }
                if (!token.IsCancellationRequested)
                {
                    await notification.SendTurnTimeoutToPlayers(new List<string> { currentPlayerId }, "Timeout!");
                    _groupTurnIndex[roomId] = idx + 1;
                    RunNextTurn(roomId, duration);
                }
            });
        }

        public void Cancel(string roomId)
        {
            if (_tokens.TryRemove(roomId, out var cts))
                cts.Cancel();
            _groupTurnOrder.TryRemove(roomId, out _);
            _groupTurnIndex.TryRemove(roomId, out _);
        }
    }
}
