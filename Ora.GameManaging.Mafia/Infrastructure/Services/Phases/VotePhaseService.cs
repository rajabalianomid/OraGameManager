using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class VotePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext)
    {
        public override async Task<PhaseModel> Prepare(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Vote phase logic here
            return await base.Prepare(appId, roomId, phaseStatus);
        }
        public override List<RoleStatusEntity> ProcessTurn(List<RoleStatusEntity> roleStatuses)
        {
            roleStatuses.ForEach(rs =>
            {
                rs.Turn = 0;
            });
            return roleStatuses;
        }
    }
}