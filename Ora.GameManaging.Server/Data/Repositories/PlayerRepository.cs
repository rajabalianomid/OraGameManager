using Microsoft.EntityFrameworkCore;

namespace Ora.GameManaging.Server.Data.Repositories
{
    public class PlayerRepository
    {
        private readonly GameDbContext _db;
        public PlayerRepository(GameDbContext db) => _db = db;

        public async Task<PlayerEntity> AddToRoomAsync(string roomId, string connectionId, string userId, string playerName, string? role, int status)
        {
            var room = await _db.Rooms.Include(r => r.Players).FirstOrDefaultAsync(r => r.RoomId == roomId)
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
            _db.Players.Add(player);
            await _db.SaveChangesAsync();
            return player;
        }

        public async Task RemoveFromRoomAsync(string roomId, string userId)
        {
            var player = await _db.Players.FirstOrDefaultAsync(p => p.UserId == userId && p.GameRoom.RoomId == roomId);
            if (player != null)
            {
                _db.Players.Remove(player);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(string roomId, string userId, int newStatus)
        {
            var player = await _db.Players.FirstOrDefaultAsync(p => p.UserId == userId && p.GameRoom.RoomId == roomId);
            if (player != null)
            {
                player.Status = newStatus;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetRoomsForUser(string userId)
        {
            return await _db.Players
                .Where(p => p.UserId == userId)
                .Select(p => p.GameRoom.RoomId)
                .Distinct()
                .ToListAsync();
        }

        public async Task UpdatePlayerConnectionId(string roomId, string userId, string newConnectionId)
        {
            var player = await _db.Players
                .FirstOrDefaultAsync(p => p.GameRoom.RoomId == roomId && p.UserId == userId);
            if (player != null)
            {
                player.ConnectionId = newConnectionId;
                await _db.SaveChangesAsync();
            }
        }
    }
}
