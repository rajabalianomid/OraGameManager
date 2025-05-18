using Microsoft.EntityFrameworkCore;

namespace Ora.GameManaging.Server.Data.Repositories
{
    public class GameRoomRepository(GameDbContext db)
    {
        public async Task<GameRoomEntity?> GetByRoomIdAsync(string appId, string roomId)
        {
            return await db.Rooms
                .Include(r => r.Players)
                .Include(r => r.Events)
                .FirstOrDefaultAsync(r => r.AppId == appId && r.RoomId == roomId);
        }

        public async Task<List<GameRoomEntity>> GetAllAsync()
        {
            return await db.Rooms
                .Include(r => r.Players)
                .Include(r => r.Events)
                .ToListAsync();
        }

        public async Task<GameRoomEntity> CreateAsync(string appId, string roomId, int turnDuration)
        {
            var room = new GameRoomEntity
            {
                AppId = appId,
                RoomId = roomId,
                TurnDurationSeconds = turnDuration
            };
            db.Rooms.Add(room);
            await db.SaveChangesAsync();
            return room;
        }

        public async Task RemoveAsync(string appId, string roomId)
        {
            var room = await db.Rooms.FirstOrDefaultAsync(r => r.AppId == appId && r.RoomId == roomId);
            if (room != null)
            {
                db.Rooms.Remove(room);
                await db.SaveChangesAsync();
            }
        }

        public async Task SaveSnapshotAsync(string appId, string roomId, object state)
        {
            var room = await db.Rooms.FirstOrDefaultAsync(r => r.AppId == appId && r.RoomId == roomId);
            if (room != null)
            {
                room.LastSnapshotJson = System.Text.Json.JsonSerializer.Serialize(state);
                await db.SaveChangesAsync();
            }
        }
    }
}
