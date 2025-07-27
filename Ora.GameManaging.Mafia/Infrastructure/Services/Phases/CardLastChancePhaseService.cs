using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;

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
            await dbContext.SaveChangesAsync(); // Save changes to the database
            var result = await base.Prepared(appId, roomId, phaseStatus);
            return result;
        }
        public override async Task<PreparingPhaseModel> Preparing(string appId, string roomId, string phaseStatus, string playerId, int round, bool isTurn)
        {
            var preparingModel = await base.Preparing(appId, roomId, phaseStatus, playerId, round, isTurn);

            if (isTurn)
            {
                var alivePlayers = await dbContext.RoleStatuses
                .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId && rs.Health > 0).ToListAsync();


                var actionHistory = dbContext.GameActionHistories.Include(i => i.Ability).Where(w => w.ApplicationInstanceId == appId &&
                w.RoomId == roomId &&
                w.CurrentPhase == phaseStatus &&
                w.ActorUserId == playerId &&
                w.IsProcessed == false).FirstOrDefault();

                var oldActionHistoryCards = dbContext.GameActionHistories.Include(i => i.Ability)
                    .Where(w => w.ApplicationInstanceId == appId && w.RoomId == roomId && w.Ability.IsCard)
                    .Select(s => s.Ability.Name).ToList();

                var cards = await dbContext.GeneralAttributes
                    .Where(gc => gc.ApplicationInstanceId == appId && gc.EntityId == roomId && gc.EntityName == EntityKeys.RoomCard)
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                List<(GeneralAttributeEntity Value, int Index)> foundRemainingCards = [.. cards.Select((s, index) => new { value = s, index })
                    .Where(w => !oldActionHistoryCards.Contains(w.value.Key))
                    .Select(s => (s.value, Index: s.index + 1))];


                var cardsModel = new List<LastCardChanceModel>();
                if (actionHistory == null)
                {
                    cardsModel = [.. foundRemainingCards.Select(s => new LastCardChanceModel
                    {
                        Id = s.Value.Id,
                        Name = $"Card {(s.Index)}",
                        Description = string.Empty,
                        Icon = string.Empty
                    })];
                }
                else if (actionHistory != null && actionHistory.TargetUserId == string.Empty)
                {
                    var cardNames = cards.Select(s => s.Key).ToList();

                    cardsModel = [.. (await dbContext.AbilityEntities
                    .Where(w => cardNames.Any(a => a == w.Name) && (actionHistory == null || w.Id == actionHistory.AbilityId)).ToListAsync())
                    .Select((c, index) => new LastCardChanceModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Icon = c.Icon,
                        SelfAct = c.SelfAct ?? false,
                        ShowFront = true
                    })];
                    preparingModel.SelectedAbility = actionHistory.Ability.Name;
                }

                if (actionHistory != null && actionHistory.TargetUserId == string.Empty)
                    preparingModel.ActingOn = [.. alivePlayers.Where(w => cardsModel.Any(a => a.SelfAct) ? w.UserId == playerId : w.UserId != playerId).Select(s => s.UserId)];

                if (cardsModel.Count != 0 && (actionHistory != null && actionHistory.TargetUserId != string.Empty))
                    preparingModel.Information = cardsModel.Select(s => $"You selected: {s.Name} card that means: {s.Description}").FirstOrDefault() ?? string.Empty;

                preparingModel.Cards = cardsModel;
            }
            return preparingModel;
        }

        public override async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            if (roleStatuses.Any(a => a.TempVoteCount > 0))
            {
                var topCandidates = roleStatuses.GroupBy(g => g.TempVoteCount).OrderByDescending(o => o.Key).First().ToList();
                var random = new Random();
                var player = topCandidates[random.Next(topCandidates.Count)];
                if (!player.Selected)
                {
                    player.Selected = true;
                    await dbContext.SaveChangesAsync();
                }
                var result = roleStatuses.Where(w => w.UserId == player.UserId).Select((s, index) =>
                {
                    s.Turn = index;
                    return s;
                }).ToList();

                return result;
            }
            return [];
        }
    }
}
