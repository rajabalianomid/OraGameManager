using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class FinalVotePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId)
        {
            var aliveCount = await dbContext.RoleStatuses
                .CountAsync(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId && rs.Health > 0);

            var roleStatuses = await dbContext.RoleStatuses.Where(w => w.TempVoteCount >= aliveCount / 2).ToListAsync();

            var result = new PreparingPhaseModel
            {
                ActingOn = [.. roleStatuses.Select(a => a.UserId)],
                HasVideo = false // Enable video for Talk phase
            };
            return result;
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
        public override List<RoleStatusEntity> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            //if (roleStatuses == null || !roleStatuses.Any())
            //{
            //    throw new ArgumentException("Role statuses cannot be null or empty.", nameof(roleStatuses));
            //}

            //var previousPhase = phase.GetPreviousPhaseName();

            //var actionHistories = dbContext.GameActionHistories.Where(ga => ga.ApplicationInstanceId == roleStatuses.First().ApplicationInstanceId &&
            //ga.RoomId == roleStatuses.First().RoomId &&
            //ga.Phase == previousPhase && ga.Round == round).ToList();

            //actionHistories.GroupBy(g => g.TargetUserId).ToList().ForEach(g =>
            //{
            //    var targetUserId = g.Key;
            //    var voteCount = g.Count();
            //    var roleStatus = roleStatuses.FirstOrDefault(rs => rs.UserId == targetUserId);
            //    if (roleStatus != null)
            //    {
            //        roleStatus.VoteCount += voteCount;
            //    }
            //});


            List<RoleStatusEntity> result = new();
            var votedRoleStatuses = roleStatuses.Where(w => w.TempVoteCount >= roleStatuses.Count / 2).Select((s, index) => { s.Turn = index; return s.UserId; }).ToList();
            if (votedRoleStatuses.Count > 0)
            {
                result = [.. roleStatuses.Where(w => !votedRoleStatuses.Any(a => a == w.UserId))];
                //DBContext.RoleStatuses.Where(w => result.Any(a => a.UserId == w.UserId))
                //    .ExecuteUpdateAsync(setters => setters.SetProperty(rs => rs.ActingOnMe, true));
            }
            return result;
        }
    }
}