using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class TurnManager(NotificationManager notification)
    {
        private readonly NotificationManager _notification = notification;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _tokens = new();

        public void StartTurn(
            string roomId,
            Func<string, (string nextPlayerId, string nextPlayerName)> getNextPlayer,
            int duration)
        {
            if (_tokens.TryGetValue(roomId, out var oldCts))
                oldCts.Cancel();

            var cts = new CancellationTokenSource();
            _tokens[roomId] = cts;
            var token = cts.Token;

            var (playerId, playerName) = getNextPlayer(roomId);

            _ = Task.Run(async () =>
            {
                await _notification.SendTurnChanged(roomId, playerName);

                for (int i = duration; i >= 0; i--)
                {
                    if (token.IsCancellationRequested) return;
                    await _notification.SendTimerTick(roomId, i);
                    await Task.Delay(1000, token);
                }

                if (!token.IsCancellationRequested)
                {
                    await _notification.SendTurnTimeout(roomId, playerName);
                    StartTurn(roomId, getNextPlayer, duration);
                }
            });
        }

        public void Cancel(string roomId)
        {
            if (_tokens.TryRemove(roomId, out var cts))
                cts.Cancel();
        }
    }
}
