using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class ActionPhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId, int round, bool isTurn)
        {
            var model = await base.Preparing(appId, roomId, phaseStatus, playerId, round, isTurn);

            if (isTurn)
            {
                var alivePlayers = await dbContext.RoleStatuses
                    .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId && rs.Health > 0).ToListAsync();

                var foundPlayer = alivePlayers.FirstOrDefault(rs => rs.UserId == playerId) ?? throw new InvalidOperationException($"Player with ID {playerId} not found in room {roomId}.");
                
                if (foundPlayer?.SelfAbilityCount == 0)
                    alivePlayers.Remove(foundPlayer); // Remove player if they have no self abilities


                model.ActingOn = foundPlayer?.Lock == false ? [.. alivePlayers.Select(a => a.UserId)] : [];
                model.Information = foundPlayer?.Lock == true ? "You locked" : "";
            }
            model.HasVideo = true;// Enable video for Talk phase
            return model;
        }
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Action phase logic here
            return await base.Prepared(appId, roomId, phaseStatus);
        }
    }
}