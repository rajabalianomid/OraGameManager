using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class NotificationManager(IHubContext<GameHub> hubContext)
    {

        // Notify only selected players that it's their turn.
        public Task SendTurnChangedToPlayers(List<string> playerConnectionIds, string message = "It's your turn!")
        {
            var tasks = playerConnectionIds
                .Select(id => hubContext.Clients.Client(id).SendAsync("TurnChanged", message))
                .ToList();
            return Task.WhenAll(tasks);
        }
        public Task SendTurnChangedToPlayers(List<string> playerConnectionIds, GameRoom gameRoom, string message = "It's your turn!")
        {
            var tasks = playerConnectionIds
                .Select(id => hubContext.Clients.Client(id).SendAsync($"TurnChanged:{gameRoom.GetPlayerInfo(id)}", message))
                .ToList();
            return Task.WhenAll(tasks);
        }

        // Send timer ticks only to selected players.
        public Task SendTimerTickToPlayers(List<string> playerConnectionIds, int seconds)
        {
            var tasks = playerConnectionIds
                .Select(id => hubContext.Clients.Client(id).SendAsync("TimerTick", seconds))
                .ToList();
            return Task.WhenAll(tasks);
        }
        //public Task SendTimerTickToPlayers(List<string> playerConnectionIds, GameRoom gameRoom, int seconds)
        //{
        //    var tasks = playerConnectionIds
        //        .Select(id => hubContext.Clients.Client(id).SendAsync($"TimerTick:{gameRoom.GetPlayerInfo(id)}", seconds))
        //        .ToList();
        //    return Task.WhenAll(tasks);
        //}

        // Send turn timeout only to selected players.
        public Task SendTurnTimeoutToPlayers(List<string> playerConnectionIds, string message = "Timeout!")
        {
            var tasks = playerConnectionIds
                .Select(id => hubContext.Clients.Client(id).SendAsync("TurnTimeout", message))
                .ToList();
            return Task.WhenAll(tasks);
        }

        public Task SendPlayerJoined(string roomId, string playerName)
            => hubContext.Clients.Group(roomId).SendAsync("PlayerJoined", playerName);

        public Task SendPlayerLeft(string roomId, string playerName)
            => hubContext.Clients.Group(roomId).SendAsync("PlayerLeft", playerName);

        public Task SendTimerPausedToPlayers(List<string> playerConnectionIds)
        {
            var tasks = playerConnectionIds
                .Select(id => hubContext.Clients.Client(id).SendAsync("TimerPaused"))
                .ToList();
            return Task.WhenAll(tasks);
        }

        public Task SendTimerResumedToPlayers(List<string> playerConnectionIds)
        {
            var tasks = playerConnectionIds
                .Select(id => hubContext.Clients.Client(id).SendAsync("TimerResumed"))
                .ToList();
            return Task.WhenAll(tasks);
        }

        // Notify the entire group that group turn is finished.
        public Task SendGroupTurnEnded(string roomId)
            => hubContext.Clients.Group(roomId).SendAsync("GroupTurnEnded", "Group turn has finished!");

    }
}
