using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;

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

            bool isCard = false;
            if (abilityName.StartsWith(EntityKeys.UnknowCardPrefix) && int.TryParse(abilityName.Split(' ').Last(), out int cardIndex))
            {
                var cards = await dbContext.GeneralAttributes
                            .Where(gc => gc.ApplicationInstanceId == applicationInstanceId && gc.EntityId == roomId && gc.EntityName == EntityKeys.RoomCard)
                            .OrderBy(x => x.Id)
                            .ToListAsync();

                List<(GeneralAttributeEntity Value, int Index)> foundCards = [.. cards.Select((s, index) => new { value = s, index })
                    .Select(s => (s.value, Index: s.index + 1))];

                abilityName = foundCards.Where(w => w.Index == cardIndex).Select(s => s.Value.Key).FirstOrDefault() ?? string.Empty;
                isCard = true;
            }
            //if (isCard == false)
            //    isCard = await dbContext.AbilityEntities.AnyAsync(w => w.Name.Equals(abilityName) && w.IsCard == true);

            var foundAbility = await dbContext.AbilityEntities
                               .FirstOrDefaultAsync(a => a.Name == abilityName && a.RelatedPhase == phase && (isCard == true || a.IsCard == true || a.RoleStatusesAbilities.Any(a => a.RoleStatus.UserId == userId)))
                               ?? throw new Exception($"Ability '{abilityName}' not found for user '{userId}' in phase '{phase}'.");

            // Check if a similar GameActionHistory already exists
            var existingAction = await dbContext.GameActionHistories.FirstOrDefaultAsync(a =>
                a.ApplicationInstanceId == applicationInstanceId &&
                a.RoomId == roomId &&
                a.ActorUserId == userId &&
                a.IsProcessed == false &&
                a.Round == round &&
                a.CurrentPhase == phase);

            if (existingAction != null)
            {
                if (string.IsNullOrEmpty(existingAction.TargetUserId) && !string.IsNullOrEmpty(targetUserId))
                {
                    existingAction.TargetUserId = targetUserId;
                    await dbContext.SaveChangesAsync();
                }
                // Optionally, you can update or skip
                return;
            }
            var actorRole = await dbContext.RoleStatuses.Where(w => w.UserId == userId).Select(s => s.RoleName).FirstOrDefaultAsync();
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
                Force = foundAbility.ForceAction,
                ActorRole = actorRole ?? "Unknown",
            };
            // Add the action to the database
            dbContext.GameActionHistories.Add(action);
            await dbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(string applicationInstanceId, string roomId, string userId, string abilityName, string targetUserId, float round, string phase)
        {
            // Create a new GameActionHistory entry
            var foundActionHistory = await dbContext.GameActionHistories.Include(i => i.Ability).Where(w => w.ApplicationInstanceId == applicationInstanceId &&
            w.RoomId == roomId &&
            w.IsProcessed == false &&
            w.Round == round &&
            w.CurrentPhase == phase &&
            w.Ability.Name == abilityName).FirstOrDefaultAsync();

            if (foundActionHistory is null || foundActionHistory.Ability is null)
            {
                throw new Exception($"Ability '{abilityName}' not found for user '{userId}' in phase '{phase}'.");
            }

            var abilities = await dbContext.AbilityEntities.Where(w => w.Pattern.StartsWith(foundActionHistory.Ability.Pattern) && w.ParentId != null).ToListAsync();
            foreach (var ability in abilities)
            {
                var action = new GameActionHistoryEntity
                {
                    ApplicationInstanceId = applicationInstanceId,
                    RoomId = roomId,
                    ActorUserId = userId,
                    TargetUserId = targetUserId,
                    AbilityId = ability.Id,
                    Round = round,
                    IsProcessed = false,
                    ActionTime = DateTime.UtcNow,
                    Phase = phase.GetNextPhaseName(),
                    CurrentPhase = phase,
                    Force = ability.PreparingPhase,
                    ActorRole = foundActionHistory.ActorRole ?? "Unknown",
                };
                // Add the action to the database
                dbContext.GameActionHistories.Add(action);
            }
            foundActionHistory.TargetUserId = targetUserId;

            await dbContext.SaveChangesAsync();
        }
    }
}
