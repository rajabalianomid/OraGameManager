using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class TalkPhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext)
    {
        public override async Task<PhaseModel> Prepare(string appId, string roomId, string phaseStatus, RoleStatusModel? roleStatus)
        {
            // TODO: Add Talk phase logic here
            var result = await base.Prepare(appId, roomId, phaseStatus, null);
            result.HasVideo = true; // Enable video for Talk phase
            return result;
        }
    }
}