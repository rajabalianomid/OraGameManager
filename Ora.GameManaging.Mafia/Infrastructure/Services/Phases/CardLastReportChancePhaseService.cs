using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class CardLastReportChancePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            var roleStatuses = await RoleStatusEntities(appId, roomId);
            roleStatuses.ForEach(rs =>
            {
                rs.TempVoteCount = rs.VoteCount; // Store the current vote count temporarily
                rs.VoteCount = 0;
            });
            var topCandidates = roleStatuses.GroupBy(g => g.TempVoteCount).OrderByDescending(o => o.Key).First().ToList();
            var random = new Random();
            var selected = topCandidates[random.Next(topCandidates.Count)];
            selected.Health = 0;
            return await base.Prepared(appId, roomId, phaseStatus);
        }
    }
}
