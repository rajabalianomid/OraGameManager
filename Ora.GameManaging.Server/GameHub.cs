using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Infrastructure;
using Ora.GameManaging.Server.Models;
using System.Collections.Concurrent;
using System.Numerics;

namespace Ora.GameManaging.Server
{
    public class GameHub(TurnManager turnManager, NotificationManager notification) : Hub
    {
        private static ConcurrentDictionary<string, GameRoom> Rooms = new();

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Disconnected: {Context.ConnectionId}");
            await LeaveAllRooms(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateRoom(string roomId)
        {
            if (Rooms.ContainsKey(roomId))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} already exists.");
                return;
            }

            Rooms[roomId] = new GameRoom(roomId);
            Console.WriteLine($"Room created: {roomId}");
            await Clients.All.SendAsync("RoomCreated", roomId);
        }

        public async Task DeleteRoom(string roomId)
        {
            if (Rooms.TryRemove(roomId, out var removedRoom))
            {
                turnManager.Cancel(roomId);
                foreach (var player in removedRoom.Players.Values)
                {
                    await Groups.RemoveFromGroupAsync(player.ConnectionId, roomId);
                }
                Console.WriteLine($"Room deleted: {roomId}");
                await Clients.All.SendAsync("RoomDeleted", roomId);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found.");
            }
        }

        public async Task ListRooms()
        {
            var roomList = Rooms.Keys.ToList();
            await Clients.Caller.SendAsync("ReceiveRoomList", roomList);
        }

        public async Task JoinRoom(string roomId, string playerName, string role)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }

            if (room.Players.ContainsKey(Context.ConnectionId))
            {
                await Clients.Caller.SendAsync("Error", "You are already in this room.");
                return;
            }

            var player = new PlayerInfo { ConnectionId = Context.ConnectionId, Name = playerName, Role = role };
            room.Players.TryAdd(Context.ConnectionId, player);

            Console.WriteLine($"Player {playerName} ({role}) joined room {roomId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await notification.SendPlayerJoined(roomId, playerName);
        }

        public async Task LeaveRoom(string roomId)
        {
            if (!Rooms.TryGetValue(roomId, out var room)) return;

            if (!room.Players.TryRemove(Context.ConnectionId, out var player)) return;

            Console.WriteLine($"Player {player.Name} left room {roomId}");
            await notification.SendPlayerLeft(roomId, player.Name);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task GetPlayersInRoom(string roomId)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }

            var players = room.Players.Values
                .Select(p => new { name = p.Name, status = p.Status.ToString(), role = p.Role })
                .ToList();
            await Clients.Caller.SendAsync("ReceivePlayerList", players);
        }

        public async Task UpdateStatus(string roomId, PlayerStatus newStatus)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }

            if (room.Players.TryGetValue(Context.ConnectionId, out var player))
            {
                player.Status = newStatus;
                Console.WriteLine($"Player {player.Name} updated status to {newStatus} in room {roomId}");
                await Clients.Group(roomId).SendAsync("PlayerStatusUpdated", player.Name, newStatus.ToString());
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "You are not in this room.");
            }
        }

        public async Task SendMessageToRoom(string roomId, string message)
        {
            if (!Rooms.TryGetValue(roomId, out var room)) return;
            if (!room.Players.TryGetValue(Context.ConnectionId, out var player)) return;

            Console.WriteLine($"Message from {player.Name} in room {roomId}: {message}");
            await Clients.Group(roomId).SendAsync("ReceiveMessage", player.Name, message);
        }

        // Simultaneous group turn (all at once)
        public Task StartGroupTurn(string roomId, string? roleOrConnIds)
        {
            if (!Rooms.TryGetValue(roomId, out var room) || room.Players.Count == 0)
                return Task.CompletedTask;

            List<string> groupPlayers;
            if (string.IsNullOrWhiteSpace(roleOrConnIds))
            {
                groupPlayers = room.Players.Keys.ToList();
            }
            else
            {
                var items = roleOrConnIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                groupPlayers = room.Players.Values
                    .Where(p => items.Contains(p.Role, StringComparer.OrdinalIgnoreCase) || items.Contains(p.ConnectionId))
                    .Select(p => p.ConnectionId)
                    .ToList();
            }

            if (groupPlayers.Count == 0)
            {
                Console.WriteLine("No player found for selected role(s) or connection ids.");
                return Task.CompletedTask;
            }

            turnManager.StartGroupTurnSimultaneous(roomId, groupPlayers, room.TurnDurationSeconds);
            return Task.CompletedTask;
        }

        // Rotating group turn (one by one)
        public Task StartGroupTurnRotating(string roomId, string? roleOrConnIds)
        {
            if (!Rooms.TryGetValue(roomId, out var room) || room.Players.Count == 0)
                return Task.CompletedTask;

            List<string> groupPlayers;
            if (string.IsNullOrWhiteSpace(roleOrConnIds))
            {
                groupPlayers = room.Players.Keys.ToList();
            }
            else
            {
                var items = roleOrConnIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                groupPlayers = room.Players.Values
                    .Where(p => items.Contains(p.Role, StringComparer.OrdinalIgnoreCase) || items.Contains(p.ConnectionId))
                    .Select(p => p.ConnectionId)
                    .ToList();
            }

            if (groupPlayers.Count == 0)
            {
                Console.WriteLine("No player found for selected role(s) or connection ids.");
                return Task.CompletedTask;
            }

            turnManager.StartGroupTurnRotating(roomId, groupPlayers, room.TurnDurationSeconds);
            return Task.CompletedTask;
        }

        private async Task LeaveAllRooms(string connectionId)
        {
            foreach (var room in Rooms.Values)
            {
                if (room.Players.TryRemove(connectionId, out var player))
                {
                    await Groups.RemoveFromGroupAsync(connectionId, room.RoomId);
                    await notification.SendPlayerLeft(room.RoomId, player.Name);
                }
            }
        }
    }
}
