using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class FinalVotePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext)
    {
        private readonly MafiaDbContext _dbcontext = dbContext;

        public override async Task<PhaseModel> Prepare(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add FinalVote phase logic here
            var roleStatuses = await RoleStatusEntities(appId, roomId);
            roleStatuses.ForEach(rs =>
            {
                rs.VoteCount = 0;
            });
            await _dbcontext.SaveChangesAsync(); // Save changes to the database
            return await base.Prepare(appId, roomId, phaseStatus);
        }
        public override List<RoleStatusEntity> ProcessTurn(List<RoleStatusEntity> roleStatuses)
        {
            List<RoleStatusEntity> result = new();
            var votedRoleStatuses = roleStatuses.Where(w => w.VoteCount >= roleStatuses.Count / 2).Select((s, index) => { s.Turn = index; return s; }).ToList();
            if (votedRoleStatuses.Count > 0)
            {
                result = [.. roleStatuses.Where(w => !votedRoleStatuses.Any(a => a.UserId == w.UserId))];
                DBContext.RoleStatuses.Where(w => result.Any(a => a.UserId == w.UserId))
                    .ExecuteUpdateAsync(setters => setters.SetProperty(rs => rs.ActingOnMe, true));
            }
            return result;
        }
    }
}