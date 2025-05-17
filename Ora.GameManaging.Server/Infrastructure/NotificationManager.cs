using Microsoft.AspNetCore.SignalR;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class NotificationManager
    {
        private readonly IHubContext<GameHub> _hubContext;
        public NotificationManager(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendTurnChanged(string roomId, string playerName)
            => _hubContext.Clients.Group(roomId).SendAsync("TurnChanged", playerName);

        public Task SendTimerTick(string roomId, int seconds)
            => _hubContext.Clients.Group(roomId).SendAsync("TimerTick", seconds);

        public Task SendTurnTimeout(string roomId, string playerName)
            => _hubContext.Clients.Group(roomId).SendAsync("TurnTimeout", playerName);

        public Task SendPlayerJoined(string roomId, string playerName)
            => _hubContext.Clients.Group(roomId).SendAsync("PlayerJoined", playerName);

        public Task SendPlayerLeft(string roomId, string playerName)
            => _hubContext.Clients.Group(roomId).SendAsync("PlayerLeft", playerName);
    }
}
