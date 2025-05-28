using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure.Proxy;
using Ora.GameManaging.Server.Models;
using Ora.GameManaging.Server.Models.Adapter;
using System.Collections.Concurrent;

namespace Ora.GameManaging.Server.Infrastructure
{
    /// <summary>
    /// Handles all game lifecycle logic, including game start, player management, and turn management.
    /// </summary>
    public class GameManager(
        GameRoomRepository roomRepo,
        PlayerRepository playerRepo,
        EventRepository eventRepo,
        NotificationManager notification,
        TurnManager turnManager,
        GrpcAdapter grpcAdapter)
    {

        // Key: AppId:RoomId
        internal static ConcurrentDictionary<string, GameRoom> Rooms = new();

        /// <summary>
        /// Should be called after a player joins a room. Checks if the game should start.
        /// </summary>
        public async Task OnPlayerJoinedAsync(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
                return;

            // Retrieve MaxPlayer from the database or cache
            int maxPlayer = await grpcAdapter.Do<int, MaxRoomPlayerModel>(new MaxRoomPlayerModel { ApplicationInstanceId = appId, RoomId = roomId });

            if (room.Players.Count == maxPlayer)
            {
                // Start the game if the required number of players is reached
                await StartGameAsync(appId, roomId, room);
            }
        }

        /// <summary>
        /// Starts the game and triggers the first rotating group turn.
        /// </summary>
        private async Task StartGameAsync(string appId, string roomId, GameRoom room)
        {
            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null) return;

            dbRoom.IsGameStarted = true;
            await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());
            await eventRepo.AddAsync(dbRoom.Id, "GameStarted", null, null);

            // Start the first rotating group turn
            var playerIds = room.Players.Values.Select(p => p.ConnectionId).ToList();
            turnManager.StartGroupTurnRotating($"{appId}:{roomId}", playerIds, room.TurnDurationSeconds);

            await notification.SendTurnChangedToPlayers(playerIds, room, "Game started! First turn.");
        }
    }
}