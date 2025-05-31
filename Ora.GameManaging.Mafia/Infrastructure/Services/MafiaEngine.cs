using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;
using System.Text.Json;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class MafiaEngine(MafiaDbContext dbContext)
    {
        // This method should fetch role actions from the database or any data source
        private List<RoleActionModel> GetRoleActions(string roleName, string roomId)
        {
            var expression = dbContext.GeneralAttributes
                                .Where(ga =>
                                    ga.EntityName == EntityKeys.GameRoom &&
                                    ga.Key == $"{roleName}_Expression" &&
                                    ga.EntityId == roomId)
                                .Select(ga => ga.Value)
                                .FirstOrDefault() ?? string.Empty;

            return [new RoleActionModel { RoleName = roleName, Expression = expression }];
        }

        public async Task<LatestInformationResponseModel> PrepareLatestInformationAsync(string requestModel)
        {
            var model = JsonSerializer.Deserialize<LatestInformationRequestModel>(requestModel)
                ?? throw new ArgumentNullException(nameof(requestModel), "Request model cannot be null");

            // Example: Find the target player (in production, fetch from model or database)
            var targetPlayer = model.Players.Where(w => w.UserId == model.CurrentTurnPlayerId).FirstOrDefault()
                ?? throw new ArgumentException($"Player with UserId {model.CurrentTurnPlayerId} not found in the request model.", nameof(requestModel)); // Updated paramName to 'requestModel'

            var lastPartUserId = model.CurrentTurnPlayerId.Split(":").Last();
            var roleStatusEntity = await dbContext.RoleStatuses
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(rs =>
                                                rs.ApplicationInstanceId == model.AppId &&
                                                rs.UserId == lastPartUserId &&
                                                rs.RoomId == targetPlayer.RoomId &&
                                                rs.RoleName == targetPlayer.Role);
            if (roleStatusEntity is not null)
            {
                targetPlayer.RoleStatus = new RoleStatusModel
                {
                    RoleName = roleStatusEntity.RoleName,
                    Health = roleStatusEntity.Health,
                    AbilityCount = roleStatusEntity.AbilityCount,
                    SelfAbilityCount = roleStatusEntity.SelfAbilityCount,
                    HasNightAbility = roleStatusEntity.HasNightAbility,
                    HasDayAbility = roleStatusEntity.HasDayAbility,
                    CanSpeak = roleStatusEntity.CanSpeak,
                    DarkSide = roleStatusEntity.DarkSide,
                    Abilities = roleStatusEntity.Abilities
                };
            }

            // Example: Assume the current role is Doctor
            var roleActions = GetRoleActions(targetPlayer.Role, targetPlayer.RoomId);

            foreach (var action in roleActions)
            {
                var globals = new ScriptGlobals { target = targetPlayer };

                await CSharpScript.EvaluateAsync(
                    action.Expression, ScriptOptions.Default.AddReferences(typeof(PlayerInfoModel).Assembly).AddImports("System"),
                    globals);
            }

            // At this point, targetPlayer.Health is updated if the action was executed
            await Task.CompletedTask;
            return new LatestInformationResponseModel();
        }
    }
}
