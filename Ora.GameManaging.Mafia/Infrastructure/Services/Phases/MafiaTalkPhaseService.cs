using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class MafiaTalkPhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        private readonly MafiaDbContext _dbcontext = dbContext;

        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Talk phase logic here
            var result = await base.Prepared(appId, roomId, phaseStatus);
            return result;
        }
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId)
        {
            var result = await base.Preparing(appId, roomId, phaseStatus, playerId);
            result.HasVideo = true; // Enable video for Talk phase
            return result;
        }
        public override async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            _dbcontext.RoleStatuses.Where(w => roleStatuses.Any(a => a.UserId == w.UserId))
                .ToList()
                .ForEach(rs =>
                {
                    rs.VoteCount = 0; // Reset turn for all role statuses
                });
            await _dbcontext.SaveChangesAsync(); // Save changes to the database

            var turns = roleStatuses.Where(w => w.DarkSide).ToList();
            turns.ForEach(rs =>
            {
                rs.Turn = 0;
            });
            return turns;
        }
    }
}