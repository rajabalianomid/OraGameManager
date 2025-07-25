﻿using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using System;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class BasePhaseService(MafiaDbContext dbContext)
    {
        protected readonly MafiaDbContext DBContext = dbContext ?? throw new NullReferenceException("dbContext");

        public virtual async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Defense phase logic here
            var actions = await GetNotProcessedActions(appId, roomId);
            var abilities = await GetAbilities();
            var roleStatuses = await RoleStatusEntities(appId, roomId);


            await ProcessAction(actions, abilities, roleStatuses, phaseStatus);

            var phaseModel = new PhaseModel
            {
                Actions = [.. actions.Select(a => new ActionHistoryModel(a))],
                Abilities = [.. abilities.Select(a => new AbilityModel(a))],
                RoleStatuses = [.. roleStatuses.Select(a => new RoleStatusModel(a))],
                Result = [.. actions.Select(s => s.Result)]
            };
            return phaseModel;
        }
        public virtual async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId)
        {
            await Task.CompletedTask; // Placeholder for async method signature
            return new PreparingPhaseModel
            {
                ActingOn = []
            };
        }
        public virtual async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            await Task.CompletedTask; // Placeholder for async method signature
            return roleStatuses;
        }
        protected async Task<List<GameActionHistoryEntity>> GetNotProcessedActions(string appId, string roomId) =>
            await dbContext.GameActionHistories
            .Include(ga => ga.Ability)
            .Where(ga => ga.ApplicationInstanceId == appId && ga.RoomId == roomId && !ga.IsProcessed)
            .ToListAsync();
        protected async Task<List<AbilityEntity>> GetAbilities() =>
            await dbContext.AbilityEntities
            .ToListAsync();
        protected async Task<List<RoleStatusEntity>> RoleStatusEntities(string appId, string roomId) =>
            await dbContext.RoleStatuses
            .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId)
            .ToListAsync();
        protected async Task ChangeAllActing(string appId, string roomId, bool state) =>
                       await dbContext.RoleStatuses.Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(rs => rs.ActingOnMe, state));

        private async Task ProcessAction(List<GameActionHistoryEntity> actions, List<AbilityEntity> abilities, List<RoleStatusEntity> roleStatuses, string phaseStatus)
        {
            if (actions.Count > 0)
            {
                try
                {
                    // Handle case where there are no actions to process
                    foreach (var action in actions)
                    {
                        //var nextPhase = action.Ability.RelatedPhase?.GetNextPhaseName();
                        if (!string.IsNullOrEmpty(action.Ability.Expression) && action.Ability.RelatedPhase == phaseStatus)
                        {
                            var globals = new ScriptGlobals { Abilities = abilities, Actions = actions, RoleStatuses = roleStatuses, Action = action };

                            action.Result = await CSharpScript.EvaluateAsync<string>(
                                action.Ability.Expression, ScriptOptions.Default
                                .AddReferences(typeof(PlayerInfoModel).Assembly)
                                .AddImports("System", "System.Linq"), // <-- Add "System.Linq"
                                globals);
                        }
                        if (action.Ability.RelatedPhase == phaseStatus)
                            action.IsProcessed = true; // Mark as processed
                    }
                    await dbContext.SaveChangesAsync(); // Save changes to the database
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
