using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System.Threading.Tasks;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class LobbyPhaseService(MafiaDbContext dbContext) : BasePhaseService(dbContext ?? throw new NullReferenceException("dbContext"))
    {
        public override async Task<PhaseModel> Prepared(string appId, string roomId, string phaseStatus)
        {
            // TODO: Add Lobby phase logic here
            await ChangeAllActing(appId, roomId, false);

            return await base.Prepared(appId, roomId, phaseStatus);
        }
        public override async Task<List<RoleStatusEntity>> ProcessTurn(List<RoleStatusEntity> roleStatuses, string phase, float round)
        {
            var outOfTurn = roleStatuses.Where(w => w.Turn < 0).ToList();
            if (outOfTurn.Count > 0)
            {
                outOfTurn.ForEach(f => f.Turn = f.Turn * -1);
                await dbContext.SaveChangesAsync();
            }

            roleStatuses.ForEach(rs =>
            {
                rs.Turn = 0;
            });
            await Task.CompletedTask;
            return roleStatuses;
        }
    }
}