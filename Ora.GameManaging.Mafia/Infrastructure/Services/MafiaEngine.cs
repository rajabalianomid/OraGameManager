using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Infrastructure.Services.Phases;
using Ora.GameManaging.Mafia.Model;
using System.Text.Json;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class MafiaEngine(MafiaDbContext dbContext, PhaseServiceFactory phaseServiceFactory)
    {
        public async Task<LatestInformationResponseModel> PrepareLatestInformationAsync(string requestModel)
        {
            // Deserialize the request model from JSON
            var model = JsonSerializer.Deserialize<LatestInformationRequestModel>(requestModel)
                ?? throw new ArgumentNullException(nameof(requestModel), "Request model cannot be null");

            // Find the current turn player
            var targetPlayer = model.Players.FirstOrDefault(w => w.UserId == model.CurrentTurnPlayerId)
                ?? throw new ArgumentException($"Player with UserId {model.CurrentTurnPlayerId} not found in the request model.", nameof(requestModel));

            // Get all role statuses for this room and application
            var allRoleStatuses = await dbContext.RoleStatuses
                .AsNoTracking()
                .Where(rs =>
                    rs.ApplicationInstanceId == model.AppId &&
                    rs.RoomId == targetPlayer.RoomId)
                .ToListAsync();


            //This Part is for filling the RoleStatus from Mafia rolestatuse table for each player that is comming from the request model
            var statusLookup = allRoleStatuses
                .ToDictionary(
                    rs => (rs.UserId, rs.RoleName),
                    rs => rs
                );

            foreach (var player in model.Players)
            {
                var userIdPart = player.UserId.Split(':').Last();
                if (statusLookup.TryGetValue((userIdPart, player.Role), out var statusEntity))
                {
                    player.RoleStatus = new RoleStatusModel
                    {
                        RoleName = statusEntity.RoleName,
                        Health = statusEntity.Health,
                        AbilityCount = statusEntity.AbilityCount,
                        SelfAbilityCount = statusEntity.SelfAbilityCount,
                        HasNightAbility = statusEntity.HasNightAbility,
                        HasDayAbility = statusEntity.HasDayAbility,
                        CanSpeak = statusEntity.CanSpeak,
                        DarkSide = statusEntity.DarkSide,
                        Abilities = statusEntity.Abilities,
                        Challenge = statusEntity.Challenge
                    };
                }
                else
                {
                    player.RoleStatus = null!;
                }
            }

            // Find the RoleStatus for the current turn player
            var lastPartUserId = model.CurrentTurnPlayerId.Split(":").Last();
            var roleStatusEntity = allRoleStatuses
                .FirstOrDefault(rs => rs.UserId == lastPartUserId && rs.RoleName == targetPlayer.Role);

            //Map to RoleStatusModel
            RoleStatusModel? roleStatus = null;
            if (roleStatusEntity is not null)
            {
                roleStatus = new RoleStatusModel
                {
                    RoleName = roleStatusEntity.RoleName,
                    Health = roleStatusEntity.Health,
                    AbilityCount = roleStatusEntity.AbilityCount,
                    SelfAbilityCount = roleStatusEntity.SelfAbilityCount,
                    HasNightAbility = roleStatusEntity.HasNightAbility,
                    HasDayAbility = roleStatusEntity.HasDayAbility,
                    CanSpeak = roleStatusEntity.CanSpeak,
                    DarkSide = roleStatusEntity.DarkSide,
                    Abilities = roleStatusEntity.Abilities,
                    Challenge = roleStatusEntity.Challenge
                };
            }

            // Parse abilities from the Abilities column
            var abilityNames = (roleStatus?.Abilities ?? string.Empty)
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Get only abilities related to the current phase
            var phase = model.Phase;
            var abilities = await dbContext.Set<AbilityEntity>()
                .Where(a => abilityNames.Contains(a.Name) && a.RelatedPhase == phase)
                .ToListAsync();

            var phaseService = phaseServiceFactory.GetPhaseService(phase);
            await phaseService.Prepare(model.AppId, model.RoomId, phase);

            // Build the response model
            var response = new LatestInformationResponseModel
            {
                Data = new LatestInformationDataResponseModel
                {
                    Phase = phase,
                    Round = model.TurnDurationSeconds,
                    CanSpeak = roleStatus?.CanSpeak ?? false,
                    RoleStatus = roleStatus,
                    Abilities = [.. abilities.Select(a => a.Name)],
                    AlivePlayers = [.. model.Players.Where(w => w.IsAlive)],
                    DeadPlayers = [.. model.Players.Where(w => !w.IsAlive)]
                },
                ExtraInfo = new ExtraInfoDetailsModel { ForceNextTurns = [.. allRoleStatuses.Where(rs => rs.Challenge).Select(s => $"{s.ApplicationInstanceId}:{s.UserId}")] }
            };

            return response;
        }

        public async Task MakeEmptyOfChallengeAsync(string appId, string roomId)
        {
            // Fetch all role statuses for the specified application and room
            var roleStatuses = await dbContext.RoleStatuses
                .Where(rs => rs.ApplicationInstanceId == appId && rs.RoomId == roomId)
                .ToListAsync();
            // Update the Challenge property to an empty string for each role status
            foreach (var status in roleStatuses)
            {
                status.Challenge = false;
            }
            // Save changes to the database
            await dbContext.SaveChangesAsync();
        }

        public async Task ProcessedActions(string appId, string roomId)
        {
            var findNotProcessedActions = await dbContext.GameActionHistories.Where(w => w.ApplicationInstanceId == appId && w.RoomId == roomId && w.IsProcessed == false).ToListAsync();
            if (findNotProcessedActions.Count > 0)
            {
                foreach (var action in findNotProcessedActions)
                {
                    action.IsProcessed = true;
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetNightReportAsync(string appId, string roomId, float nightRound)
        {
            // Fetch all actions for the specified night that are processed
            var nightActions = await dbContext.GameActionHistories
                .Include(a => a.Ability)
                .Where(a =>
                    a.ApplicationInstanceId == appId &&
                    a.RoomId == roomId &&
                    a.Round == nightRound &&
                    a.Ability.IsNightAbility)
                .ToListAsync();

            var report = new List<string>();

            foreach (var action in nightActions)
            {
                // Example: "Mafia (user123) tried to Kill (ability) Doctor (user456): Success"
                var description = $"{action.ActorRole} ({action.ActorUserId}) used {action.Ability.Name} on {action.TargetUserId} ({action.TargetUserId})";
                if (!string.IsNullOrEmpty(action.Result))
                    description += $": {action.Result}";
                report.Add(description);
            }

            return report;
        }

        public async Task<string> GetNightKillReportAsync(string appId, string roomId, float nightRound)
        {
            // Fetch all night actions for the specified round
            var nightActions = await dbContext.GameActionHistories
                .Include(a => a.Ability)
                .Where(a =>
                    a.ApplicationInstanceId == appId &&
                    a.RoomId == roomId &&
                    a.Round == nightRound &&
                    a.Ability.IsNightAbility)
                .ToListAsync();

            // Count only successful kills
            var killCount = nightActions
                .Count(a => a.Ability.Name == "Kill" && (a.Result == null || a.Result.ToLower() == "success"));

            if (killCount == 0)
                return "There were no kills last night.";
            if (killCount == 1)
                return "There was 1 kill last night.";
            return $"There were {killCount} kills last night.";
        }

        public NextPhaseModel GetNextPhase(string currentPhase)
        {
            var isLastPhase = ((PhaseStatus)Enum.GetValues<PhaseStatus>().Length - 1).ToString() == currentPhase;
            if (!Enum.TryParse<PhaseStatus>(currentPhase, out var phase))
                return new NextPhaseModel { Name = PhaseStatus.Lobby.ToString(), IsLastPhase = isLastPhase };

            int next = ((int)phase + 1) % Enum.GetValues<PhaseStatus>().Length;
            return new NextPhaseModel { Name = ((PhaseStatus)next).ToString(), IsLastPhase = isLastPhase };
        }
    }
}
