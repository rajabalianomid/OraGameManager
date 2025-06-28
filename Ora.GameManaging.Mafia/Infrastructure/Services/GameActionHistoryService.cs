using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class GameActionHistoryService(MafiaDbContext dbContext)
    {
        public async Task Insert(string applicationInstanceId, string roomId, string userId, string abilityName, string targetUserId, float round, string phase)
        {
            // Create a new GameActionHistory entry
            var foundAbility = await dbContext.AbilityEntities.Include(i => i.RoleStatusesAbilities).ThenInclude(a => a.RoleStatus)
                .FirstOrDefaultAsync(a => a.Name == abilityName && a.RelatedPhase == phase && a.RoleStatusesAbilities.Any(a => a.RoleStatus.UserId == userId));

            if (foundAbility is null)
            {
                throw new Exception($"Ability '{abilityName}' not found for user '{userId}' in phase '{phase}'.");
            }

            var action = new GameActionHistoryEntity
            {
                ApplicationInstanceId = applicationInstanceId,
                RoomId = roomId,
                ActorUserId = userId,
                TargetUserId = targetUserId,
                AbilityId = foundAbility.Id,
                Round = round,
                IsProcessed = false,
                ActionTime = DateTime.UtcNow,
                ActorRole = foundAbility.RoleStatusesAbilities.FirstOrDefault(a => a.RoleStatus.UserId == userId)?.RoleStatus.RoleName ?? "Unknown",
            };
            // Add the action to the database
            dbContext.GameActionHistories.Add(action);
            await dbContext.SaveChangesAsync();
        }
    }
}
