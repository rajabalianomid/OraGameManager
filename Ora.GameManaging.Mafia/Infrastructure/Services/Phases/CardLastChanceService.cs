using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class CardLastChanceService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            return await base.Prepared(appId, roomId, phaseStatus);
        }
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId)
        {
            var alivePlayers = await dbContext.RoleStatuses
                .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId && rs.Health > 0).ToListAsync();

            var cards = await dbContext.GeneralAttributes
                .Where(gc => gc.ApplicationInstanceId == appId && gc.EntityId == roomId && gc.EntityName == EntityKeys.RoomCard)
                .ToListAsync();

            var cardsModel = await dbContext.AbilityEntities
                .Where(w => cards.Any(a => a.Key == w.Name))
                .Select(c => new LastCardChanceModel
                {
                    Name = c.Name,
                    Description = c.Description,
                    Icon = c.Icon,
                    SelfAct = c.SelfAct ?? false
                }).ToListAsync();

            var foundPlayer = alivePlayers.FirstOrDefault(rs => rs.UserId == playerId) ?? throw new InvalidOperationException($"Player with ID {playerId} not found in room {roomId}.");

            var result = new PreparingPhaseModel
            {
                ActingOn = [.. alivePlayers.Select(a => a.UserId)],
                HasVideo = false,
                Information = string.Empty,
                Cards = cardsModel
            };
            return result;
        }

        public override List<RoleStatusEntity> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
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
