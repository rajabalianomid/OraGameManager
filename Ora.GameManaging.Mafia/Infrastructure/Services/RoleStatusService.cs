using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class RoleStatusService(MafiaDbContext dbContext)
    {
        public async Task<List<string>> GetTurnsAsync(string applicationInstanceId, string roomId)
        {
            // Fetch all role statuses for the room
            var roleStatuses = await dbContext.RoleStatuses
                .Where(rs => rs.ApplicationInstanceId == applicationInstanceId && rs.RoomId == roomId)
                .ToListAsync();
            // Get all roles that have this maximum turn
            var turns = roleStatuses
                .OrderBy(rs => rs.Turn)
                .Select(rs => $"{applicationInstanceId}:{rs.UserId}")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            return turns;
        }
    }
}
