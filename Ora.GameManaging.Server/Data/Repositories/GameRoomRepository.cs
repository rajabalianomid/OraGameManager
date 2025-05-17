using Microsoft.EntityFrameworkCore;

namespace Ora.GameManaging.Server.Data.Repositories
{
    public class GameRoomRepository
    {
        private readonly GameDbContext _db;
        public GameRoomRepository(GameDbContext db) => _db = db;

        public async Task<GameRoomEntity?> GetByRoomIdAsync(string roomId)
        {
            return await _db.Rooms
                .Include(r => r.Players)
                .Include(r => r.Events)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public async Task<List<GameRoomEntity>> GetAllAsync()
        {
            return await _db.Rooms
                .Include(r => r.Players)
                .Include(r => r.Events)
                .ToListAsync();
        }

        public async Task<GameRoomEntity> CreateAsync(string roomId, int turnDuration)
        {
            var room = new GameRoomEntity
            {
                RoomId = roomId,
                TurnDurationSeconds = turnDuration
            };
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
            return room;
        }

        public async Task RemoveAsync(string roomId)
        {
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room != null)
            {
                _db.Rooms.Remove(room);
                await _db.SaveChangesAsync();
            }
        }

        public async Task SaveSnapshotAsync(string roomId, object state)
        {
            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room != null)
            {
                room.LastSnapshotJson = System.Text.Json.JsonSerializer.Serialize(state);
                await _db.SaveChangesAsync();
            }
        }
    }
}
