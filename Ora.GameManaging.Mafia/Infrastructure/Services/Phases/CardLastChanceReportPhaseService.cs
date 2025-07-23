using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class CardLastChanceReportPhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            var roleStatuses = await RoleStatusEntities(appId, roomId);
            roleStatuses.ForEach(rs =>
            {
                rs.TempVoteCount = 0;
                rs.VoteCount = 0;
                rs.Selected = false;
            });
            await dbContext.SaveChangesAsync(); // Save changes to the database
            return await base.Prepared(appId, roomId, phaseStatus);
        }
    }
}
