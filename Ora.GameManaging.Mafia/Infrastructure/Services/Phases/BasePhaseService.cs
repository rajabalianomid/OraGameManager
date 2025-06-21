using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class BasePhaseService(MafiaDbContext dbContext)
    {
        public virtual async Task<PhaseModel> Prepare(string appId, string roomId, string phaseStatus, RoleStatusModel? roleStatus)
        {
            // TODO: Add Defense phase logic here
            var actions = await GetNotProcessedActions(appId, roomId);
            var abilities = await GetAbilities();
            var roleStatuses = await RoleStatusEntities(appId, roomId);


            if (actions.Count > 0)
            {
                // Handle case where there are no actions to process
                foreach (var action in actions)
                {
                    if (!string.IsNullOrEmpty(action.Ability.Expression) || action.Ability.RelatedPhase == phaseStatus)
                    {
                        var globals = new ScriptGlobals { Abilities = abilities, Actions = actions, RoleStatuses = roleStatuses };

                        action.Result = await CSharpScript.EvaluateAsync<string>(
                            action.Ability.Expression, ScriptOptions.Default
                            .AddReferences(typeof(PlayerInfoModel).Assembly)
                            .AddImports("System", "System.Linq"), // <-- Add "System.Linq"
                            globals);
                    }
                    action.IsProcessed = true; // Mark as processed
                    dbContext.GameActionHistories.Update(action);
                }

                await dbContext.SaveChangesAsync(); // Save changes to the database
            }
            var phaseModel = new PhaseModel
            {
                Actions = [.. actions.Select(a => new ActionHistoryModel(a))],
                Abilities = [.. abilities.Select(a => new AbilityModel(a))],
                RoleStatuses = [.. roleStatuses.Select(a => new RoleStatusModel(a))],
                Result = [.. actions.Select(s => s.Result)]
            };
            return phaseModel;
        }
        public virtual List<RoleStatusEntity> ProcessTurn(List<RoleStatusEntity> roleStatuses)
        {
            return roleStatuses;
        }

        protected async Task<List<GameActionHistoryEntity>> GetNotProcessedActions(string appId, string roomId) =>
            await dbContext.GameActionHistories
            .AsNoTracking()
            .Include(ga => ga.Ability)
            .Where(ga => ga.ApplicationInstanceId == appId && ga.RoomId == roomId && !ga.IsProcessed)
            .ToListAsync();

        protected async Task<List<AbilityEntity>> GetAbilities() =>
            await dbContext.AbilityEntities
            .AsNoTracking()
            .ToListAsync();

        protected async Task<List<RoleStatusEntity>> RoleStatusEntities(string appId, string roomId) =>
            await dbContext.RoleStatuses
            .AsNoTracking()
            .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId)
            .ToListAsync();
    }
}
