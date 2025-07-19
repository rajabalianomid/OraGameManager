using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;
using System.Linq;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class CardLastChancePhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            var roleStatuses = await RoleStatusEntities(appId, roomId);
            roleStatuses.ForEach(rs =>
            {
                rs.TempVoteCount = rs.VoteCount; // Store the current vote count temporarily
                rs.VoteCount = 0;
            });
            var result = await base.Prepared(appId, roomId, phaseStatus);
            return result;
        }
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId)
        {
            var alivePlayers = await dbContext.RoleStatuses
                .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId && rs.Health > 0).ToListAsync();

            var actionHistory = dbContext.GameActionHistories.Where(w => w.ApplicationInstanceId == appId &&
            w.RoomId == roomId &&
            w.Phase == phaseStatus &&
            w.IsProcessed == false).FirstOrDefault();

            var cards = await dbContext.GeneralAttributes
                .Where(gc => gc.ApplicationInstanceId == appId && gc.EntityId == roomId && gc.EntityName == EntityKeys.RoomCard)
                .ToListAsync();

            var cardsModel = new List<LastCardChanceModel>();
            if (actionHistory == null)
            {
                cardsModel = [.. cards.Select((c, index) => new LastCardChanceModel
                {
                    Name = $"Card {(index+1)}" ,
                    Description = string.Empty,
                    Icon = string.Empty
                })];
            }
            else if (actionHistory != null && actionHistory.TargetUserId == string.Empty)
            {
                cardsModel = await dbContext.AbilityEntities
                    .Where(w => cards.Any(a => a.Key == w.Name) && (actionHistory == null || w.Id == actionHistory.AbilityId))
                    .Select((c, index) => new LastCardChanceModel
                    {
                        Name = c.Name,
                        Description = c.Description,
                        Icon = c.Icon,
                        SelfAct = c.SelfAct ?? false
                    }).ToListAsync();
            }


            //var foundPlayer = alivePlayers.FirstOrDefault(rs => rs.UserId == playerId) ?? throw new InvalidOperationException($"Player with ID {playerId} not found in room {roomId}.");

            var result = new PreparingPhaseModel
            {
                ActingOn = actionHistory == null || actionHistory.TargetUserId == string.Empty ? [] : [.. alivePlayers.Where(w => cardsModel.Any(a => a.SelfAct) ? w.UserId == playerId : w.UserId != playerId).Select(s => s.UserId)],
                HasVideo = false,
                Information = cardsModel.Any() && (actionHistory != null && actionHistory.TargetUserId != string.Empty) ? cardsModel.Select(s => $"You selected: {s.Name} card that means: {s.Description}").FirstOrDefault() ?? string.Empty : string.Empty,
                Cards = actionHistory == null ? cardsModel : []
            };
            return result;
        }

        public override async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            var result = roleStatuses.GroupBy(g => g.VoteCount).OrderByDescending(o => o.Key).Take(1).SelectMany((sm, index) =>
            {
                _ = sm.Select(s =>
                {
                    s.Turn = index;
                    return s;
                });
                return sm;
            }).ToList();
            await Task.CompletedTask;
            return result;
        }
    }
}
