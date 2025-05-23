using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Server.Data;

namespace Ora.GameManaging.Server.Data.Repositories
{
    public class RoomRepository(GameDbContext db)
    {
        public async Task<bool> AddAsync(GameRoomEntity room, CancellationToken cancellationToken = default)
        {
            db.Rooms.Add(room);
            return await db.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(string roomId, CancellationToken cancellationToken = default)
        {
            var room = await db.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId, cancellationToken);
            if (room == null)
                return false;
            db.Rooms.Remove(room);
            return await db.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<List<GameRoomEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await db.Rooms
                .Include(r => r.Players)
                .Include(r => r.Events)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<GameRoomEntity>> GetByAppIdAsync(string appId, int pageSize, int skip, bool count, CancellationToken cancellationToken = default)
        {
            return await db.Rooms
                .Include(r => r.Players)
                .Include(r => r.Events)
                .Where(r => r.AppId == appId)
                .OrderBy(o => o.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<GameRoomEntity?> GetByIdAsync(string roomId, CancellationToken cancellationToken = default)
        {
            return await db.Rooms
                .Include(r => r.Players)
                .Include(r => r.Events)
                .FirstOrDefaultAsync(r => r.RoomId == roomId, cancellationToken);
        }

        public async Task<bool> UpdateAsync(GameRoomEntity room, CancellationToken cancellationToken = default)
        {
            db.Rooms.Update(room);
            return await db.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}