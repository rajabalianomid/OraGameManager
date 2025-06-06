﻿using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class RoleStatusService(MafiaDbContext dbContext)
    {
        public async Task<List<object>> GetTurnsAsync(string applicationInstanceId, string roomId)
        {
            // Fetch all role statuses for the room
            var roleStatuses = await dbContext.RoleStatuses
                .Where(rs => rs.ApplicationInstanceId == applicationInstanceId && rs.RoomId == roomId)
                .ToListAsync();

            // Group by Turn value
            var grouped = roleStatuses
                .GroupBy(rs => rs.Turn)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var users = g.Select(rs => $"{applicationInstanceId}:{rs.UserId}").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    // Explicitly cast to object
                    return users.Count == 1 ? (object)users[0] : (object)users;
                })
                .ToList();

            return grouped;
        }
    }
}
