using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class VotePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId)
        {
            var roleStatuses = await dbContext.RoleStatuses.Where(w => w.ApplicationInstanceId == appId && w.RoomId == roomId && w.Health > 0).ToListAsync();
            var result = new PreparingPhaseModel
            {
                ActingOn = [.. roleStatuses.Where(w => w.UserId != playerId).Select(a => a.UserId)]
            };
            return result;
        }
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Vote phase logic here
            return await base.Prepared(appId, roomId, phaseStatus);
        }
        public override async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            roleStatuses.ForEach(rs =>
            {
                rs.Turn = 0;
            });
            await Task.CompletedTask;
            return roleStatuses;
        }
    }
}