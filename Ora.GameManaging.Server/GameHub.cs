using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Data.Migrations;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure;
using Ora.GameManaging.Server.Models;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;
using static Ora.GameManaging.Mafia.Protos.SettingGrpc;

namespace Ora.GameManaging.Server
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GameHub : Hub
    {
        // Key: AppId:RoomId
        private static ConcurrentDictionary<string, GameRoom> Rooms = new();

        private readonly GrpcClientFactory _clientFactory;
        private readonly GameRoomRepository _roomRepo;
        private readonly PlayerRepository _playerRepo;
        private readonly EventRepository _eventRepo;
        private readonly NotificationManager _notification;
        private readonly TurnManager _turnManager;

        public GameHub(
            GrpcClientFactory clientFactory,
            GameRoomRepository roomRepo,
            PlayerRepository playerRepo,
            EventRepository eventRepo,
            NotificationManager notification,
            TurnManager turnManager)
        {
            _clientFactory = clientFactory;
            _roomRepo = roomRepo;
            _playerRepo = playerRepo;
            _eventRepo = eventRepo;
            _notification = notification;
            _turnManager = turnManager;
        }

        // Database to memory loader
        public static async Task LoadAllRoomsAsync(GameRoomRepository repo)
        {
            var dbRooms = await repo.GetAllAsync();
            var dict = new ConcurrentDictionary<string, GameRoom>();
            foreach (var dbRoom in dbRooms)
            {
                var key = $"{dbRoom.AppId}:{dbRoom.RoomId}";
                var room = new GameRoom(dbRoom.AppId, dbRoom.RoomId)
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
                dict.TryAdd(key, room);
            }
            Rooms = dict;
        }

        public static void RemoveStalePlayersFromMemory(List<(string AppId, string RoomId, string UserId)> stalePlayers)
        {
            foreach (var (appId, roomId, userId) in stalePlayers)
            {
                var key = $"{appId}:{roomId}";
                if (Rooms.TryGetValue(key, out var room))
                {
                    room.Players.TryRemove(userId, out _);
                }
            }
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

        // Called by client to auto join all their rooms after reconnect
        public async Task Reconnect(string appId, string userId)
        {
            var connectionId = Context.ConnectionId;
            var rooms = await _playerRepo.GetRoomsForUser(appId, userId);
            foreach (var roomId in rooms)
            {
                var key = $"{appId}:{roomId}";
                await _playerRepo.UpdatePlayerConnectionId(appId, roomId, userId, connectionId);
                await _playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);
                if (Rooms.TryGetValue(key, out var room))
                {
                    if (room.Players.TryGetValue(userId, out var player))
                        player.ConnectionId = connectionId;
                }
                await Groups.AddToGroupAsync(connectionId, key);
            }
            await Clients.Caller.SendAsync("ReconnectedRooms", rooms);
        }

        public async Task CreateRoom(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            if (Rooms.ContainsKey(key))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} already exists for this client.");
                return;
            }
            var room = new GameRoom(appId, roomId);
            Rooms[key] = room;
            var dbRoom = await _roomRepo.CreateAsync(appId, roomId, room.TurnDurationSeconds);
            await _eventRepo.AddAsync(dbRoom.Id, "RoomCreated", null, null);

            Console.WriteLine($"Room created: {key}");
            await Clients.All.SendAsync("RoomCreated", roomId);
        }

        public async Task DeleteRoom(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            if (Rooms.TryRemove(key, out var removedRoom))
            {
                _turnManager.Cancel(key);
                foreach (var player in removedRoom.Players.Values)
                    await Groups.RemoveFromGroupAsync(player.ConnectionId, key);

                var dbRoom = await _roomRepo.GetByRoomIdAsync(appId, roomId);
                if (dbRoom != null)
                {
                    await _eventRepo.AddAsync(dbRoom.Id, "RoomDeleted", null, null);
                    await _roomRepo.RemoveAsync(appId, roomId);
                }
                Console.WriteLine($"Room deleted: {key}");
                await Clients.All.SendAsync("RoomDeleted", roomId);
            }
            else
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found for this client.");
        }

        public async Task ListRooms(string appId)
        {
            var roomList = Rooms.Values.Where(r => r.AppId == appId).Select(r => r.RoomId).ToList();
            await Clients.Caller.SendAsync("ReceiveRoomList", roomList);
        }

        public async Task JoinRoom(string appId, string roomId, string userId, string playerName, string role)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
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

            var dbRoom = await _roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null)
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found in DB.");
                return;
            }
            await _playerRepo.AddToRoomAsync(appId, roomId, Context.ConnectionId, userId, playerName, role, (int)PlayerStatus.Online);

            await _playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);

            await _eventRepo.AddAsync(dbRoom.Id, "PlayerJoined", playerName, role);

            await Groups.AddToGroupAsync(Context.ConnectionId, key);
            await _notification.SendPlayerJoined(key, playerName);
            await _roomRepo.SaveSnapshotAsync(appId, roomId, SerializeRoom(room));
        }

        public async Task JoinRoomAuto(string appId, string roomId, string playerName)
        {
            var userId = $"{appId}:{playerName}";
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }
            if (room.Players.ContainsKey(userId))
            {
                await Clients.Caller.SendAsync("Error", "You are already in this room.");
                return;
            }
            var roleReply = await _clientFactory.CreateClient<SettingGrpcClient>("Mafia_Setting").GetNextAvailableRoleAsync(new Mafia.Protos.GetSettingRoomByIdRequest { RoomId = roomId });
            var player = new PlayerInfo { ConnectionId = Context.ConnectionId, UserId = userId, Name = playerName, Role = roleReply.Role };
            room.Players.TryAdd(userId, player);

            var dbRoom = await _roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null)
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found in DB.");
                return;
            }
            await _playerRepo.AddToRoomAsync(appId, roomId, Context.ConnectionId, userId, playerName, roleReply.Role, (int)PlayerStatus.Online);

            await _playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);

            await _eventRepo.AddAsync(dbRoom.Id, "PlayerJoined", playerName, roleReply.Role);

            await Groups.AddToGroupAsync(Context.ConnectionId, key);
            await _notification.SendPlayerJoined(key, playerName);
            await _roomRepo.SaveSnapshotAsync(appId, roomId, SerializeRoom(room));
        }

        public async Task LeaveRoom(string appId, string roomId, string userId)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room)) return;
            if (!room.Players.TryRemove(userId, out var player)) return;

            var dbRoom = await _roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null) return;
            await _playerRepo.RemoveFromRoomAsync(appId, roomId, userId);
            await _eventRepo.AddAsync(dbRoom.Id, "PlayerLeft", player.Name, player.Role);

            await _notification.SendPlayerLeft(key, player.Name);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, key);
            await _roomRepo.SaveSnapshotAsync(appId, roomId, SerializeRoom(room));
        }

        public async Task GetPlayersInRoom(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }
            var players = room.Players.Values.Select(p => new { name = p.Name, status = p.Status.ToString(), role = p.Role }).ToList();
            await Clients.Caller.SendAsync("ReceivePlayerList", players);
        }

        public async Task UpdateStatus(string appId, string roomId, string userId, PlayerStatus newStatus)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }
            if (room.Players.TryGetValue(userId, out var player))
            {
                player.Status = newStatus;
                await Clients.Group(key).SendAsync("PlayerStatusUpdated", player.Name, newStatus.ToString());

                var dbRoom = await _roomRepo.GetByRoomIdAsync(appId, roomId);
                if (dbRoom == null) return;
                await _playerRepo.UpdateStatusAsync(appId, roomId, userId, (int)newStatus);
                await _eventRepo.AddAsync(dbRoom.Id, "StatusChanged", player.Name, newStatus.ToString());
                await _playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);
                await _roomRepo.SaveSnapshotAsync(appId, roomId, SerializeRoom(room));
            }
            else
                await Clients.Caller.SendAsync("Error", "You are not in this room.");
        }

        public async Task SendMessageToRoom(string appId, string roomId, string userId, string message)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room)) return;
            if (!room.Players.TryGetValue(userId, out var player)) return;

            await Clients.Group(key).SendAsync("ReceiveMessage", player.Name, message);

            var dbRoom = await _roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom != null)
                await _eventRepo.AddAsync(dbRoom.Id, "ChatMessage", player.Name, message);

            await _playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);
        }

        // 1. Start group turn (simultaneous)
        public async Task StartGroupTurn(string appId, string roomId, string? rolesOrConnectionIds)
        {
            var key = $"{appId}:{roomId}";
            // Parse playerIds: (this logic should be as per your game logic)
            var playerIds = GetTargetPlayerConnectionIds(appId, roomId, rolesOrConnectionIds);
            int duration = 30; // Or dynamic as per room settings
            _turnManager.StartGroupTurnSimultaneous(key, playerIds, duration);
            await Clients.Group(key).SendAsync("TurnChanged", "Group turn started (simultaneous)");
        }

        // 2. Pause group timer
        public async Task PauseGroupTimer(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            _turnManager.PauseGroupTimer(key);
            await Clients.Group(key).SendAsync("TimerPaused");
        }

        // 3. Resume group timer
        public async Task ResumeGroupTimer(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            _turnManager.ResumeGroupTimer(key);
            await Clients.Group(key).SendAsync("TimerResumed");
        }

        // 4. Start group turn rotating
        public async Task StartGroupTurnRotating(string appId, string roomId, string? rolesOrConnectionIds)
        {
            var key = $"{appId}:{roomId}";
            var playerIds = GetTargetPlayerConnectionIds(appId, roomId, rolesOrConnectionIds);
            int duration = 30; // Or dynamic as per room settings
            _turnManager.StartGroupTurnRotating(key, playerIds, duration);
            await Clients.Group(key).SendAsync("TurnChanged", "Group turn started (rotating)");
        }

        public async Task ChangePlayerRole(string appId, string roomId, string userId, string newRole)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return;
            }
            if (!room.Players.TryGetValue(userId, out var player))
            {
                await Clients.Caller.SendAsync("Error", "You are not in this room.");
                return;
            }

            // Update in-memory role
            player.Role = newRole;

            // Update role in the database
            await _playerRepo.UpdateRoleAsync(appId, roomId, userId, newRole);

            // Log the event
            var dbRoom = await _roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom != null)
                await _eventRepo.AddAsync(dbRoom.Id, "RoleChanged", player.Name, newRole);

            // Notify all room members about the role change
            await Clients.Group(key).SendAsync("PlayerRoleChanged", player.Name, newRole);

            // Save room snapshot
            await _roomRepo.SaveSnapshotAsync(appId, roomId, SerializeRoom(room));
        }

        // Helper method to extract connection IDs from input (customize as needed)
        private List<string> GetTargetPlayerConnectionIds(string appId, string roomId, string? rolesOrConnectionIds)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
                return new List<string>();

            if (string.IsNullOrWhiteSpace(rolesOrConnectionIds))
                return room.Players.Values.Select(p => p.ConnectionId).ToList();

            var tokens = rolesOrConnectionIds.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            // Match either by role or direct connectionId
            return room.Players.Values
                .Where(p => tokens.Contains(p.Role, StringComparer.OrdinalIgnoreCase) || tokens.Contains(p.ConnectionId))
                .Select(p => p.ConnectionId)
                .ToList();
        }

        private static string SerializeRoom(GameRoom room)
        {
            return JsonSerializer.Serialize(new
            {
                AppId = room.AppId,
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
                    var key = $"{room.AppId}:{room.RoomId}";
                    await Groups.RemoveFromGroupAsync(connectionId, key);
                    var dbRoom = await _roomRepo.GetByRoomIdAsync(room.AppId, room.RoomId);
                    if (dbRoom != null)
                    {
                        await _playerRepo.RemoveFromRoomAsync(room.AppId, room.RoomId, player.UserId);
                        await _eventRepo.AddAsync(dbRoom.Id, "PlayerLeft", player.Name, player.Role);
                        await _roomRepo.SaveSnapshotAsync(room.AppId, room.RoomId, SerializeRoom(room));
                    }
                }
            }
        }
    }
}
