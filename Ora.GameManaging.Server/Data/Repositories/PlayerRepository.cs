using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Server.Data.Migrations;
using Ora.GameManaging.Server.Models;

namespace Ora.GameManaging.Server.Data.Repositories
{
    public class PlayerRepository(GameDbContext db)
    {
        public async Task<PlayerEntity> AddToRoomAsync(string appId, string roomId, string connectionId, string userId, string playerName, string? role, int status, List<GeneralAttributeEntity> extraInfos)
        {
            var room = await db.Rooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.AppId == appId && r.RoomId == roomId)
                ?? throw new Exception("Room not found");


            db.Players.RemoveRange(db.Players.Where(p => p.UserId == userId).ToList());
            db.GeneralAttributes.RemoveRange(db.GeneralAttributes.Where(a => a.EntityKey == "Players" && a.EntityId == userId).ToList());
            await db.SaveChangesAsync();

            var player = new PlayerEntity
            {
                ConnectionId = connectionId,
                UserId = userId,
                Name = playerName,
                Role = role,
                Status = status,
                GameRoom = room
            };
            db.GeneralAttributes.AddRange(extraInfos);
            db.Players.Add(player);
            await db.SaveChangesAsync();
            return player;
        }

        public async Task RemoveFromRoomAsync(string appId, string roomId, string userId)
        {
            var player = await db.Players
                .Include(p => p.GameRoom)
                .FirstOrDefaultAsync(p => p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId && p.UserId == userId);
            if (player != null)
            {
                db.Players.Remove(player);
                db.GeneralAttributes.RemoveRange(db.GeneralAttributes.Where(a => a.EntityKey == "Players" && a.EntityId == userId).ToList());
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(string appId, string roomId, string userId, int newStatus)
        {
            var player = await db.Players
                .Include(p => p.GameRoom)
                .FirstOrDefaultAsync(p => p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId && p.UserId == userId);
            if (player != null)
            {
                player.Status = newStatus;
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetRoomsForUser(string appId, string userId)
        {
            return await db.Players
                .Where(p => p.UserId == userId && p.GameRoom.AppId == appId)
                .Select(p => p.GameRoom.RoomId)
                .Distinct()
                .ToListAsync();
        }

        public async Task UpdatePlayerConnectionId(string appId, string roomId, string userId, string newConnectionId)
        {
            var player = await db.Players
                .Include(p => p.GameRoom)
                .FirstOrDefaultAsync(p => p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId && p.UserId == userId);
            if (player != null)
            {
                player.ConnectionId = newConnectionId;
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateLastSeenAsync(string appId, string roomId, string userId, DateTime lastSeen)
        {
            var player = await db.Players
                .Include(p => p.GameRoom)
                .FirstOrDefaultAsync(p => p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId && p.UserId == userId);
            if (player != null)
            {
                player.LastSeen = lastSeen;
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<(string AppId, string RoomId, string UserId)>> RemoveStalePlayersAsync(DateTime cutoffUtc)
        {
            var stalePlayers = await db.Players.Include(p => p.GameRoom)
                .Where(p => p.LastSeen < cutoffUtc)
                .ToListAsync();

            var removed = new List<(string, string, string)>();
            foreach (var player in stalePlayers)
            {
                removed.Add((player.GameRoom.AppId, player.GameRoom.RoomId, player.UserId));
                db.GeneralAttributes.RemoveRange(db.GeneralAttributes.Where(a => a.EntityKey == "Players" && a.EntityId == player.UserId).ToList());
                db.Players.Remove(player);
            }

            if (stalePlayers.Count > 0)
                await db.SaveChangesAsync();

            return removed;
        }

        public async Task UpdateRoleAsync(string appId, string roomId, string userId, string newRole)
        {
            var player = await db.Players
                .Include(p => p.GameRoom)
                .FirstOrDefaultAsync(p => p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId && p.UserId == userId);
            if (player != null)
            {
                player.Role = newRole;
                await db.SaveChangesAsync();
            }
        }

        public async Task AddExtraInfoAsync(List<GeneralAttributeEntity> entities)
        {
            db.GeneralAttributes.AddRange(entities);
            await db.SaveChangesAsync();
        }
    }
}
