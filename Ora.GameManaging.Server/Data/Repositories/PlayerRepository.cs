using Microsoft.EntityFrameworkCore;

namespace Ora.GameManaging.Server.Data.Repositories
{
    public class PlayerRepository(GameDbContext db)
    {
        public async Task<PlayerEntity> AddToRoomAsync(string appId, string roomId, string connectionId, string userId, string playerName, string? role, int status)
        {
            var room = await db.Rooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.AppId == appId && r.RoomId == roomId)
                ?? throw new Exception("Room not found");

            var player = new PlayerEntity
            {
                ConnectionId = connectionId,
                UserId = userId,
                Name = playerName,
                Role = role,
                Status = status,
                GameRoom = room
            };
            db.Players.Add(player);
            await db.SaveChangesAsync();
            return player;
        }

        public async Task RemoveFromRoomAsync(string appId, string roomId, string userId)
        {
            var player = await db.Players
                .FirstOrDefaultAsync(p => p.UserId == userId && p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId);
            if (player != null)
            {
                db.Players.Remove(player);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(string appId, string roomId, string userId, int newStatus)
        {
            var player = await db.Players
                .FirstOrDefaultAsync(p => p.UserId == userId && p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId);
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
                .FirstOrDefaultAsync(p => p.GameRoom.AppId == appId && p.GameRoom.RoomId == roomId && p.UserId == userId);
            if (player != null)
            {
                player.ConnectionId = newConnectionId;
                await db.SaveChangesAsync();
            }
        }
    }
}
