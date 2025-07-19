using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class GameActionHistoryService(MafiaDbContext dbContext)
    {
        public async Task<List<GameActionHistoryEntity>> GetAsync(string applicationInstanceId, string roomId, string userId, List<int> abilityIds, float round, string phase)
        {
            var existingActions = await dbContext.GameActionHistories.Include(i => i.Ability).Where(a =>
                a.ApplicationInstanceId == applicationInstanceId &&
                a.RoomId == roomId &&
                a.ActorUserId == userId &&
                abilityIds.Contains(a.AbilityId) &&
                a.Round == round &&
                a.CurrentPhase == phase).ToListAsync();
            return existingActions;
        }
        public async Task InsertAsync(string applicationInstanceId, string roomId, string userId, string abilityName, string targetUserId, float round, string phase)
        {
            // Create a new GameActionHistory entry
            var foundAbility = await dbContext.AbilityEntities.Include(i => i.RoleStatusesAbilities).ThenInclude(a => a.RoleStatus)
                .FirstOrDefaultAsync(a => a.Name == abilityName && a.RelatedPhase == phase && a.RoleStatusesAbilities.Any(a => a.RoleStatus.UserId == userId));

            if (foundAbility is null)
            {
                throw new Exception($"Ability '{abilityName}' not found for user '{userId}' in phase '{phase}'.");
            }
            // Check if a similar GameActionHistory already exists
            var existingAction = await dbContext.GameActionHistories.FirstOrDefaultAsync(a =>
                a.ApplicationInstanceId == applicationInstanceId &&
                a.RoomId == roomId &&
                a.ActorUserId == userId &&
                a.AbilityId == foundAbility.Id &&
                a.Round == round &&
                a.CurrentPhase == phase);

            if (existingAction != null)
            {
                // Optionally, you can update or skip
                return;
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
                Phase = phase.GetNextPhaseName(),
                CurrentPhase = phase,
                ActorRole = foundAbility.RoleStatusesAbilities.FirstOrDefault(a => a.RoleStatus.UserId == userId)?.RoleStatus.RoleName ?? "Unknown",
            };
            // Add the action to the database
            dbContext.GameActionHistories.Add(action);
            await dbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(string applicationInstanceId, string roomId, string userId, string abilityName, string targetUserId, float round, string phase)
        {
            // Create a new GameActionHistory entry
            var foundAbility = await dbContext.AbilityEntities.Include(i => i.RoleStatusesAbilities).ThenInclude(a => a.RoleStatus)
                .FirstOrDefaultAsync(a => a.Name == abilityName && a.RelatedPhase == phase && a.RoleStatusesAbilities.Any(a => a.RoleStatus.UserId == userId));

            if (foundAbility is null)
            {
                throw new Exception($"Ability '{abilityName}' not found for user '{userId}' in phase '{phase}'.");
            }
            // Check if a similar GameActionHistory already exists
            var existingAction = await dbContext.GameActionHistories.FirstOrDefaultAsync(a =>
                a.ApplicationInstanceId == applicationInstanceId &&
                a.RoomId == roomId &&
                a.ActorUserId == userId &&
                a.AbilityId == foundAbility.Id &&
                a.Round == round &&
                a.CurrentPhase == phase);

            if (existingAction != null)
            {
                existingAction.TargetUserId = targetUserId;
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
