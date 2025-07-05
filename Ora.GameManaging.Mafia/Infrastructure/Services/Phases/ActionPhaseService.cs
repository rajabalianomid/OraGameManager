using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class ActionPhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId)
        {
            var alivePlayers = await dbContext.RoleStatuses
                .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId && rs.Health > 0).ToListAsync();

            var foundPlayer = alivePlayers.FirstOrDefault(rs => rs.UserId == playerId);

            if (foundPlayer == null)
                throw new InvalidOperationException($"Player with ID {playerId} not found in room {roomId}.");

            if (foundPlayer?.SelfAbilityCount == 0)
                alivePlayers.Remove(foundPlayer); // Remove player if they have no self abilities

            var result = new PreparingPhaseModel
            {
                ActingOn = foundPlayer?.Lock == false ? [.. alivePlayers.Select(a => a.UserId)] : [],
                HasVideo = true, // Enable video for Talk phase
                Information = foundPlayer?.Lock == true ? "You locked" : "",
            };
            return result;
        }
        public override async Task<PhaseModel> Prepare(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Action phase logic here
            return await base.Prepare(appId, roomId, phaseStatus);
        }
    }
}