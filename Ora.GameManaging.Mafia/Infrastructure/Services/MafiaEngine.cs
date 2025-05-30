using DynamicExpresso;
using Ora.GameManaging.Mafia.Model;
using System.Numerics;
using System.Text.Json;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class MafiaEngine
    {
        // This method should fetch role actions from the database or any data source
        private List<RoleActionModel> GetRoleActions(string roleName)
        {
            // Sample data for demonstration; in production, fetch from database
            return
            [
                new RoleActionModel
                {
                    RoleName = "Doctor",
                    Expression = "target.Health + 1",
                    Condition = "target.IsAlive",
                    PropertyName = "Health"
                }
            ];
        }

        public async Task<LatestInformationResponseModel> PrepareLatestInformationAsync(string requestModel)
        {
            var model = JsonSerializer.Deserialize<LatestInformationRequestModel>(requestModel)
                ?? throw new ArgumentNullException(nameof(requestModel), "Request model cannot be null");

            // Example: Find the target player (in production, fetch from model or database)
            var targetPlayer = new PlayerInfoModel { Name = "Ali", Health = 1 };

            // Example: Assume the current role is Doctor
            var roleActions = GetRoleActions("Doctor");

            var interpreter = new Interpreter();

            foreach (var action in roleActions)
            {
                // Evaluate the condition before executing the action
                var conditionResult = interpreter.SetVariable("target", targetPlayer)
                                                 .Eval<bool>(action.Condition);

                if (conditionResult)
                {
                    // Execute the action expression dynamically
                    var newValue = interpreter.SetVariable("target", targetPlayer)
                                   .Eval(action.Expression);

                    // Set the property dynamically using reflection
                    var property = typeof(PlayerInfoModel).GetProperty(action.PropertyName);
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(targetPlayer, Convert.ChangeType(newValue, property.PropertyType));
                    }
                }
            }

            // At this point, targetPlayer.Health is updated if the action was executed
            await Task.CompletedTask;
            return new LatestInformationResponseModel();
        }
    }
}
