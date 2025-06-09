using Microsoft.AspNetCore.SignalR;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure;
using Ora.GameManaging.Server.Infrastructure.Proxy;
using Ora.GameManaging.Server.Models;
using Ora.GameManaging.Server.Models.Adapter;
using System.Collections.Concurrent;
using System.Text.Json;
using static Ora.GameManaging.Server.Infrastructure.GameManager; // Import static members of GameManager

namespace Ora.GameManaging.Server
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GameHub(
        GrpcAdapter adapterFactory,
        GameRoomRepository roomRepo,
        PlayerRepository playerRepo,
        EventRepository eventRepo,
        NotificationManager notification,
        TurnManager turnManager,
        GameManager gameManager) : Hub
    {
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
                    TurnDurationSeconds = dbRoom.TurnDurationSeconds,
                    Phase = dbRoom.Phase,
                    Round = dbRoom.Round,
                    CurrentTurnPlayerId = dbRoom.CurrentTurnPlayer
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
            var rooms = await playerRepo.GetRoomsForUser(appId, userId);
            foreach (var roomId in rooms)
            {
                var key = $"{appId}:{roomId}";
                await playerRepo.UpdatePlayerConnectionId(appId, roomId, userId, connectionId);
                await playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);

                if (Rooms.TryGetValue(key, out var room))
                {
                    if (!room.Players.TryGetValue(userId, out var player))
                    {
                        var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
                        var dbPlayer = dbRoom?.Players.FirstOrDefault(p => p.UserId == userId);
                        if (dbPlayer != null)
                        {
                            player = new PlayerInfo
                            {
                                ConnectionId = connectionId,
                                UserId = dbPlayer.UserId,
                                Name = dbPlayer.Name,
                                Role = dbPlayer.Role,
                                Status = (PlayerStatus)dbPlayer.Status
                            };
                            room.Players.TryAdd(userId, player);
                        }
                    }
                    else
                    {
                        player.ConnectionId = connectionId;
                    }
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
            var dbRoom = await roomRepo.CreateAsync(appId, roomId, room.TurnDurationSeconds);
            await eventRepo.AddAsync(dbRoom.Id, "RoomCreated", null, null);

            Console.WriteLine($"Room created: {key}");
            await Clients.All.SendAsync("RoomCreated", roomId);
        }

        public async Task DeleteRoom(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            if (Rooms.TryRemove(key, out var removedRoom))
            {
                turnManager.Cancel(key);
                foreach (var player in removedRoom.Players.Values)
                    await Groups.RemoveFromGroupAsync(player.ConnectionId, key);

                var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
                if (dbRoom != null)
                {
                    await eventRepo.AddAsync(dbRoom.Id, "RoomDeleted", null, null);
                    await roomRepo.RemoveAsync(appId, roomId);
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

            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null)
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found in DB.");
                return;
            }
            await playerRepo.AddToRoomAsync(appId, roomId, Context.ConnectionId, userId, playerName, role, (int)PlayerStatus.Online);

            await playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);

            await eventRepo.AddAsync(dbRoom.Id, "PlayerJoined", playerName, role);

            await Groups.AddToGroupAsync(Context.ConnectionId, key);
            await notification.SendPlayerJoined(key, playerName);
            await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());

            // Call GameManager to handle post-join logic (e.g., auto-start game)
            await gameManager.OnPlayerJoinedAsync(appId, roomId);
        }

        public async Task<bool> JoinRoomAuto(string appId, string roomId, string playerName)
        {
            var userId = $"{appId}:{playerName}";
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} does not exist.");
                return false;
            }
            if (room.Players.TryGetValue(userId, out var existingPlayer))
            {
                if (existingPlayer.ConnectionId != Context.ConnectionId)
                {
                    room.Players.TryRemove(userId, out _);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "You are already in this room.");
                    return false;
                }
            }
            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null)
            {
                await Clients.Caller.SendAsync("Error", $"Room {roomId} not found in DB.");
                return false;
            }

            var userAlreadyJoined = await adapterFactory.Do<bool, JoinSituationModel>(new JoinSituationModel { ApplicationInstanceId = appId, RoomId = roomId, UserId = playerName });
            if (dbRoom.IsGameStarted && !userAlreadyJoined)
            {
                await Clients.Caller.SendAsync("Error", "Cannot join room, game already started.");
                return false;
            }

            await RemovePlayerRoleIfGameNotStarted(room.AppId, room.RoomId, playerName, room, dbRoom.IsGameStarted);

            var role = await adapterFactory.Do<string, NextRoleModel>(new NextRoleModel { ApplicationInstanceId = appId, RoomId = roomId, UserId = playerName });
            var player = new PlayerInfo { ConnectionId = Context.ConnectionId, UserId = userId, Name = playerName, Role = role };
            room.Players.TryAdd(userId, player);


            await playerRepo.AddToRoomAsync(appId, roomId, Context.ConnectionId, userId, playerName, role, (int)PlayerStatus.Online);

            await playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);

            await eventRepo.AddAsync(dbRoom.Id, "PlayerJoined", playerName, role);

            await Groups.AddToGroupAsync(Context.ConnectionId, key);
            await notification.SendPlayerJoined(key, playerName);
            await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());

            // Call GameManager to handle post-join logic (e.g., auto-start game)
            await gameManager.OnPlayerJoinedAsync(appId, roomId);

            return true;
        }

        public async Task LeaveRoom(string appId, string roomId, string userId)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room)) return;
            if (!room.Players.TryRemove(userId, out var player)) return;

            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null) return;

            // If game still is not started remove role assignment
            await RemovePlayerRoleIfGameNotStarted(room.AppId, room.RoomId, player.Name, room, dbRoom.IsGameStarted);
            await eventRepo.AddAsync(dbRoom.Id, "PlayerLeft", player.Name, player.Role);

            await notification.SendPlayerLeft(key, player.Name);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, key);
            await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());
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

                var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
                if (dbRoom == null) return;
                await playerRepo.UpdateStatusAsync(appId, roomId, userId, (int)newStatus);
                await eventRepo.AddAsync(dbRoom.Id, "StatusChanged", player.Name, newStatus.ToString());
                await playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);
                await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());
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

            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom != null)
                await eventRepo.AddAsync(dbRoom.Id, "ChatMessage", player.Name, message);

            await playerRepo.UpdateLastSeenAsync(appId, roomId, userId, DateTime.UtcNow);
        }

        // 1. Start group turn (simultaneous)
        public async Task StartGroupTurn(string appId, string roomId, string? rolesOrConnectionIds)
        {
            var key = $"{appId}:{roomId}";
            // Parse playerIds: (this logic should be as per your game logic)
            var userIds = GetTargetPlayerConnectionIds(appId, roomId, rolesOrConnectionIds);
            int duration = 30; // Or dynamic as per room settings
            turnManager.StartGroupTurnSimultaneous(key, userIds, duration);
            await Clients.Group(key).SendAsync("TurnChanged", "Group turn started (simultaneous)");
        }

        // 2. Pause group timer
        public async Task PauseGroupTimer(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            turnManager.PauseGroupTimer(key);
            await Clients.Group(key).SendAsync("TimerPaused");
        }

        // 3. Resume group timer
        public async Task ResumeGroupTimer(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            turnManager.ResumeGroupTimer(key);
            await Clients.Group(key).SendAsync("TimerResumed");
        }

        // 4. Start group turn rotating
        public async Task StartGroupTurnRotating(string appId, string roomId, string? rolesOrConnectionIds)
        {
            var key = $"{appId}:{roomId}";
            // Example: You can build your own grouping logic here.
            // For now, treat all as singles (old behavior):
            var userIds = GetTargetPlayerConnectionIds(appId, roomId, rolesOrConnectionIds);

            // Example: If you want to group every 2 players together:
            var userOrGroups = new List<object>();
            int groupSize = 2; // Change as needed
            for (int i = 0; i < userIds.Count; i += groupSize)
            {
                var group = userIds.Skip(i).Take(groupSize).ToList();
                if (group.Count == 1)
                    userOrGroups.Add(group[0]); // single
                else
                    userOrGroups.Add(group);    // group
            }

            int duration = 30; // Or dynamic as per room settings
            turnManager.StartGroupTurnRotating(key, userOrGroups, duration);
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
            await playerRepo.UpdateRoleAsync(appId, roomId, userId, newRole);

            // Log the event
            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom != null)
                await eventRepo.AddAsync(dbRoom.Id, "RoleChanged", player.Name, newRole);

            // Notify all room members about the role change
            await Clients.Group(key).SendAsync("PlayerRoleChanged", player.Name, newRole);

            // Save room snapshot
            await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());
        }

        // Helper method to extract connection IDs from input (customize as needed)
        private List<string> GetTargetPlayerConnectionIds(string appId, string roomId, string? rolesOrUserIds)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
                return [];

            if (string.IsNullOrWhiteSpace(rolesOrUserIds))
                return room.Players.Values.Select(p => p.ConnectionId).ToList();

            var tokens = rolesOrUserIds.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            // Match either by role or direct connectionId
            return room.Players.Values
                .Where(p => tokens.Contains(p.Role, StringComparer.OrdinalIgnoreCase) || tokens.Contains(p.UserId))
                .Select(p => p.UserId)
                .ToList();
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
                    var dbRoom = await roomRepo.GetByRoomIdAsync(room.AppId, room.RoomId);
                    if (dbRoom != null)
                    {
                        await RemovePlayerRoleIfGameNotStarted(room.AppId, room.RoomId, player.Name, room, dbRoom.IsGameStarted);
                        await eventRepo.AddAsync(dbRoom.Id, "PlayerLeft", player.Name, player.Role);
                        await roomRepo.SaveSnapshotAsync(room.AppId, room.RoomId, room.Serialize());
                    }
                }
            }
        }
        // Add this new private method to GameHub
        private async Task RemovePlayerRoleIfGameNotStarted(string appId, string roomId, string userId, GameRoom room, bool IsGameStarted)
        {
            if (!IsGameStarted)
            {
                await adapterFactory.Do<string, RemoveRoleModel>(new RemoveRoleModel
                {
                    ApplicationInstanceId = appId,
                    RoomId = roomId,
                    UserId = userId
                });
            }
            await playerRepo.RemoveFromRoomAsync(room.AppId, room.RoomId, userId);
        }
    }
}
