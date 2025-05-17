using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class TurnManager(NotificationManager notification)
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _tokens = new();
        private readonly ConcurrentDictionary<string, int> _remainingSeconds = new();

        public void StartTurn(string roomId, Func<string, (string nextPlayerId, string nextPlayerName)> getNextPlayer, int duration)
        {
            if (_tokens.TryGetValue(roomId, out var oldCts))
                oldCts.Cancel();

            var cts = new CancellationTokenSource();
            _tokens[roomId] = cts;
            var token = cts.Token;

            var (playerId, playerName) = getNextPlayer(roomId);

            _remainingSeconds[roomId] = duration;

            _ = Task.Run(async () =>
            {
                await notification.SendTurnChanged(roomId, playerName);

                for (int i = duration; i >= 0; i--)
                {
                    _remainingSeconds[roomId] = i;
                    if (token.IsCancellationRequested) return;
                    await notification.SendTimerTick(roomId, i);
                    await Task.Delay(1000, token);
                }

                if (!token.IsCancellationRequested)
                {
                    await notification.SendTurnTimeout(roomId, playerName);
                    StartTurn(roomId, getNextPlayer, duration);
                }
            });
        }

        public void PauseTimer(string roomId)
        {
            if (_tokens.TryGetValue(roomId, out var cts))
            {
                cts.Cancel();
                notification.SendTimerPaused(roomId);
            }
        }

        public void ResumeTimer(string roomId, Func<string, (string playerId, string playerName)> getCurrentPlayer, int defaultDuration)
        {
            int secondsLeft = defaultDuration;
            if (_remainingSeconds.TryGetValue(roomId, out var left))
                secondsLeft = left;

            var (playerId, playerName) = getCurrentPlayer(roomId);

            var cts = new CancellationTokenSource();
            _tokens[roomId] = cts;
            var token = cts.Token;

            _ = Task.Run(async () =>
            {
                await notification.SendTimerResumed(roomId);

                for (int i = secondsLeft; i >= 0; i--)
                {
                    _remainingSeconds[roomId] = i;
                    if (token.IsCancellationRequested) return;
                    await notification.SendTimerTick(roomId, i);
                    await Task.Delay(1000, token);
                }

                if (!token.IsCancellationRequested)
                {
                    await notification.SendTurnTimeout(roomId, playerName);
                    StartTurn(roomId, getCurrentPlayer, defaultDuration);
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
