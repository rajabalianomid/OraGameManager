using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class DefensePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Talk phase logic here
            var result = await base.Prepared(appId, roomId, phaseStatus);
            return result;
        }
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId, int round, bool isTurn)
        {
            var result = await base.Preparing(appId, roomId, phaseStatus, playerId, round, isTurn);
            result.HasVideo = true; // Enable video for Talk phase
            return result;
        }
        public override async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            List<RoleStatusEntity> result = [.. roleStatuses.Where(w => w.VoteCount >= (int)Math.Ceiling(roleStatuses.Count / 2.0)).Select((s, index) => { s.Turn = index; return s; })];
            await Task.CompletedTask;
            return result;
        }
    }
}