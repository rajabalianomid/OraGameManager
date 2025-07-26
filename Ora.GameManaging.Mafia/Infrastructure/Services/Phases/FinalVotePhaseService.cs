using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class FinalVotePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId, int round, bool isTurn)
        {
            var model = await base.Preparing(appId, roomId, phaseStatus, playerId, round, isTurn);

            if (isTurn)
            {
                var aliveCount = await dbContext.RoleStatuses
                    .CountAsync(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId && rs.Health > 0);

                var roleStatuses = await dbContext.RoleStatuses.Where(w => w.TempVoteCount >= aliveCount / 2).ToListAsync();

                model.ActingOn = [.. roleStatuses.Select(a => a.UserId)];
                model.HasVideo = false; // Enable video for Talk phase
            }

            return model;
        }
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add FinalVote phase logic here
            var roleStatuses = await RoleStatusEntities(appId, roomId);
            roleStatuses.ForEach(rs =>
            {
                rs.TempVoteCount = rs.VoteCount; // Store the current vote count temporarily
                rs.VoteCount = 0;
            });
            await dbContext.SaveChangesAsync(); // Save changes to the database
            return await base.Prepared(appId, roomId, phaseStatus);
        }
        public override async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            List<RoleStatusEntity> result = new();
            var votedRoleStatuses = roleStatuses.Where(w => w.TempVoteCount >= roleStatuses.Count / 2).Select((s, index) => { s.Turn = index; return s.UserId; }).ToList();
            if (votedRoleStatuses.Count > 0)
            {
                result = [.. roleStatuses.Where(w => !votedRoleStatuses.Any(a => a == w.UserId))];
                result.ForEach(rs =>
                {
                    rs.Turn = 0;
                });
                //DBContext.RoleStatuses.Where(w => result.Any(a => a.UserId == w.UserId))
                //    .ExecuteUpdateAsync(setters => setters.SetProperty(rs => rs.ActingOnMe, true));
            }
            await Task.CompletedTask;
            return result;
        }
    }
}