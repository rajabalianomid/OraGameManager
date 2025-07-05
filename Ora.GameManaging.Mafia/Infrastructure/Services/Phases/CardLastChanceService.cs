using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class CardLastChanceService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PhaseModel> Prepare(string appId, string roomId, string phaseStatus)
        {
            return await base.Prepare(appId, roomId, phaseStatus);
        }

        public override List<RoleStatusEntity> ProcessTurn(List<RoleStatusEntity> roleStatuses)
        {
            var result = roleStatuses.GroupBy(g => g.VoteCount).OrderByDescending(o => o.Key).SelectMany((sm, index) =>
            {
                sm.Select(s =>
                {
                    s.Turn = index;
                    return s;
                });
                return sm;
            }).ToList();
            return result;
        }
    }
}
