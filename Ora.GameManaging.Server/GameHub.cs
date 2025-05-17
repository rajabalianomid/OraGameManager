using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure;
using Ora.GameManaging.Server.Models;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;

namespace Ora.GameManaging.Server
{
    public class GameHub : Hub
    {
        private static ConcurrentDictionary<string, GameRoom> Rooms = new();

        private readonly GameRoomRepository _roomRepo;
        private readonly PlayerRepository _playerRepo;
        private readonly EventRepository _eventRepo;
        private readonly NotificationManager _notification;
        private readonly TurnManager _turnManager;

        public GameHub(
            GameRoomRepository roomRepo,
            PlayerRepository playerRepo,
            EventRepository eventRepo,
            NotificationManager notification,
            TurnManager turnManager)
        {
            _roomRepo = roomRepo;
            _playerRepo = playerRepo;
            _eventRepo = eventRepo;
            _notification = notification;
            _turnManager = turnManager;
        }

        public static async Task LoadAllRoomsAsync(GameRoomRepository repo)
        {
            var dbRooms = await repo.GetAllAsync();
            var dict = new ConcurrentDictionary<string, GameRoom>();
            foreach (var dbRoom in dbRooms)
            {
                var room = new GameRoom(dbRoom.RoomId)
                {
                    TurnDurationSeconds = dbRoom.TurnDurationSeconds
                };
                foreach (var dbPlayer in dbRoom.Players)
                {
                    var player = new PlayerInfo
                    {
                        ConnectionId = dbPlayer.ConnectionId,
                        UserId = dbPlayer.UserId,
                        Name = dbPlayer.Name,
                        Role = dbPlayer.Role,
                        Status = (PlayerStatus)dbPlayer.Status
                    };
                    room.Players.TryAdd(dbPlayer.UserId, player);
                }
                dict.TryAdd(dbRoom.RoomId, room);
            }
            Rooms = dict;
        }

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

        // AUTO JOIN / RECONNECT: Called by client after connection
        public async Task Reconnect(string userId)
        {
            var connectionId = Context.ConnectionId;

            var rooms = await _playerRepo.GetRoomsForUser(userId);
            foreach (var roomId in rooms)
            {
                await _playerRepo.UpdatePlayerConnectionId(roomId, userId, connectionId);

                if (Rooms.TryGetValue(roomId, out var room))
                {
                    if (room.Players.TryGetValue(userId, out var player))
                        player.ConnectionId = connectionId;
                }
                await Groups.AddToGroupAsync(connectionId, roomId);
            }
            await Clients.Caller.SendAsync("ReconnectedRooms", rooms);
        }

        public async Task CreateRoom(string roomId)
        {
            if (Rooms.ContainsKey(roomId))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} already exists.");
                return;
            }
            var room = new GameRoom(roomId);
            Rooms[roomId] = room;
            var dbRoom = await _roomRepo.CreateAsync(roomId, room.TurnDurationSeconds);
            await _eventRepo.AddAsync(dbRoom.Id, "RoomCreated", null, null);

            Console.WriteLine($"Room created: {roomId}");
            await Clients.All.SendAsync("RoomCreated", roomId);
        }

        public async Task DeleteRoom(string roomId)
        {
            if (Rooms.TryRemove(roomId, out var removedRoom))
            {
                _turnManager.Cancel(roomId);
                foreach (var player in removedRoom.Players.Values)
                    await Groups.RemoveFromGroupAsync(player.ConnectionId, roomId);

                var dbRoom = await _roomRepo.GetByRoomIdAsync(roomId);
                if (dbRoom != null)
                {
                    await _eventRepo.AddAsync(dbRoom.Id, "RoomDeleted", null, null);
                    await _roomRepo.RemoveAsync(roomId);
                }

                Console.WriteLine($"Room deleted: {roomId}");
                await Clients.All.SendAsync("RoomDeleted", roomId);
            }
            else
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found.");
        }

        public async Task ListRooms()
        {
            var roomList = Rooms.Keys.ToList();
            await Clients.Caller.SendAsync("ReceiveRoomList", roomList);
        }

        public async Task JoinRoom(string roomId, string userId, string playerName, string role)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }
            if (room.Players.ContainsKey(userId))
            {
                await Clients.Caller.SendAsync("Error", "You are already in this room.");
                return;
            }
            var player = new PlayerInfo { ConnectionId = Context.ConnectionId, UserId = userId, Name = playerName, Role = role };
            room.Players.TryAdd(userId, player);

            var dbRoom = await _roomRepo.GetByRoomIdAsync(roomId);
            if (dbRoom == null)
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found in DB.");
                return;
            }
            await _playerRepo.AddToRoomAsync(roomId, Context.ConnectionId, userId, playerName, role, (int)PlayerStatus.Online);
            await _eventRepo.AddAsync(dbRoom.Id, "PlayerJoined", playerName, role);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await _notification.SendPlayerJoined(roomId, playerName);
            await _roomRepo.SaveSnapshotAsync(roomId, SerializeRoom(room));
        }

        public async Task LeaveRoom(string roomId, string userId)
        {
            if (!Rooms.TryGetValue(roomId, out var room)) return;
            if (!room.Players.TryRemove(userId, out var player)) return;

            var dbRoom = await _roomRepo.GetByRoomIdAsync(roomId);
            if (dbRoom == null) return;
            await _playerRepo.RemoveFromRoomAsync(roomId, userId);
            await _eventRepo.AddAsync(dbRoom.Id, "PlayerLeft", player.Name, player.Role);

            await _notification.SendPlayerLeft(roomId, player.Name);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await _roomRepo.SaveSnapshotAsync(roomId, SerializeRoom(room));
        }

        public async Task GetPlayersInRoom(string roomId)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }
            var players = room.Players.Values.Select(p => new { name = p.Name, status = p.Status.ToString(), role = p.Role }).ToList();
            await Clients.Caller.SendAsync("ReceivePlayerList", players);
        }

        public async Task UpdateStatus(string roomId, string userId, PlayerStatus newStatus)
        {
            if (!Rooms.TryGetValue(roomId, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }
            if (room.Players.TryGetValue(userId, out var player))
            {
                player.Status = newStatus;
                await Clients.Group(roomId).SendAsync("PlayerStatusUpdated", player.Name, newStatus.ToString());

                var dbRoom = await _roomRepo.GetByRoomIdAsync(roomId);
                if (dbRoom == null) return;
                await _playerRepo.UpdateStatusAsync(roomId, userId, (int)newStatus);
                await _eventRepo.AddAsync(dbRoom.Id, "StatusChanged", player.Name, newStatus.ToString());
                await _roomRepo.SaveSnapshotAsync(roomId, SerializeRoom(room));
            }
            else
                await Clients.Caller.SendAsync("Error", "You are not in this room.");
        }

        public async Task SendMessageToRoom(string roomId, string userId, string message)
        {
            if (!Rooms.TryGetValue(roomId, out var room)) return;
            if (!room.Players.TryGetValue(userId, out var player)) return;

            await Clients.Group(roomId).SendAsync("ReceiveMessage", player.Name, message);

            var dbRoom = await _roomRepo.GetByRoomIdAsync(roomId);
            if (dbRoom != null)
                await _eventRepo.AddAsync(dbRoom.Id, "ChatMessage", player.Name, message);
        }

        // ... (GroupTurn, Pause, Resume, etc. مثل قبل با استفاده از userId)

        private static string SerializeRoom(GameRoom room)
        {
            return JsonSerializer.Serialize(new
            {
                RoomId = room.RoomId,
                Players = room.Players.Values.Select(p => new { p.ConnectionId, p.UserId, p.Name, p.Role, p.Status }),
                room.TurnDurationSeconds
            });
        }

        private async Task LeaveAllRooms(string connectionId)
        {
            foreach (var room in Rooms.Values)
            {
                var player = room.Players.Values.FirstOrDefault(p => p.ConnectionId == connectionId);
                if (player != null)
                {
                    room.Players.TryRemove(player.UserId, out _);
                    await Groups.RemoveFromGroupAsync(connectionId, room.RoomId);
                    var dbRoom = await _roomRepo.GetByRoomIdAsync(room.RoomId);
                    if (dbRoom != null)
                    {
                        await _playerRepo.RemoveFromRoomAsync(room.RoomId, player.UserId);
                        await _eventRepo.AddAsync(dbRoom.Id, "PlayerLeft", player.Name, player.Role);
                        await _roomRepo.SaveSnapshotAsync(room.RoomId, SerializeRoom(room));
                    }
                }
            }
        }
    }
}
