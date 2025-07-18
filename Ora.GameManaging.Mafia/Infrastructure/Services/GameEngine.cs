using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Infrastructure.Services.Phases;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;
using System.Text.Json;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class GameEngine(MafiaDbContext dbContext, RoleStatusService roleStatusService, SettingService settingService, PhaseServiceFactory phaseServiceFactory, GameActionHistoryService gameActionHistoryService, AzureService azureService, IConfiguration configuration)
    {
        public async Task<LatestInformationResponseModel> PrepareLatestInformationAsync(string requestModel)
        {
            // Deserialize the request model from JSON
            var model = JsonSerializer.Deserialize<LatestInformationRequestModel>(requestModel)
                ?? throw new ArgumentNullException(nameof(requestModel), "Request model cannot be null");

            if (model.TargetPlayersId.Count > 1)
                throw new ArgumentException("TargetPlayersId should contain only one player.", nameof(requestModel));

            // Find the current turn player
            var targetPlayer = model.Players.FirstOrDefault(w => w.UserId == model.TargetPlayerId)
                ?? throw new ArgumentException($"Player with UserId {model.TargetPlayerId} not found in the request model.", nameof(requestModel));

            // Get all role statuses for this room and application
            var allRoleStatuses = await dbContext.RoleStatuses
                .Include(rs => rs.RoleStatusesAbilities)
                .ThenInclude(i => i.Ability)
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
                        Abilities = statusEntity.RoleStatusesAbilities.ToAbilityModels(),
                        Challenge = statusEntity.Challenge
                    };
                }
                else
                {
                    player.RoleStatus = null!;
                }
            }

            // Find the RoleStatus for the current turn player
            var lastPartUserId = model.TargetPlayerId?.Split(":").Last();
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
                    Abilities = roleStatusEntity.RoleStatusesAbilities.ToAbilityModels(),
                    Challenge = roleStatusEntity.Challenge
                };
            }


            // Get only abilities related to the current phase
            var phase = model.Phase;
            var abilities = roleStatus?.Abilities
                .Where(a => a.RelatedPhase == phase && a.IsCard == false).ToList();

            List<GameActionHistoryEntity> actionHistories = [];
            if (abilities != null && abilities.Count != 0 && lastPartUserId != null)
            {
                actionHistories = await gameActionHistoryService.GetAsync(model.AppId, model.RoomId, lastPartUserId, abilities?.Select(a => a.Id).ToList() ?? [], model.Round, phase);
            }

            abilities = abilities?.Where(w => !actionHistories.Any(a => a.AbilityId == w.Id)).ToList();

            var phaseService = phaseServiceFactory.GetPhaseService(phase);
            var preparingPhase = await phaseService.Preparing(model.AppId, model.RoomId, phase, lastPartUserId ?? string.Empty);

            var response = new LatestInformationResponseModel();

            try
            {
                // Build the response model
                response = new LatestInformationResponseModel
                {
                    Data = new LatestInformationDataResponseModel
                    {
                        UserId = model.TargetPlayerId ?? string.Empty,
                        Phase = phase,
                        Round = model.Round,
                        RoleStatus = roleStatus,
                        Abilities = model.IsYourTurn && abilities != null ? abilities : [],
                        AlivePlayers = [.. model.Players.Where(w => w.IsAlive).Select(s => (BasePlayerInfo)s)],
                        DeadPlayers = [.. model.Players.Where(w => !w.IsAlive).Select(s => (BasePlayerInfo)s)],
                        ActingOn = [.. model.Players.Where(w => preparingPhase.ActingOn.Any(a => a == w.LastPartUserId)).Select(s => (BasePlayerInfo)s)],
                        HasVideo = preparingPhase.HasVideo,
                        Cards = preparingPhase.Cards
                    },
                    ExtraInfo = new ExtraInfoDetailsModel
                    {
                        ForceNextTurns = [.. allRoleStatuses.Where(rs => rs.Challenge).Select(s => s.UserId)]
                    }
                };
            }
            catch (Exception ex)
            {

            }


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
        public async Task<bool> AlreadyJoinToRoomAsync(string applicationInstanceId, string roomId, string userId)
        {
            return await roleStatusService.AlreadyJoinToRoomAsync(applicationInstanceId, roomId, userId);
        }
        public async Task<int> GetMaximumPlayerFromRoomAsync(string applicationInstanceId, string roomId)
        {
            return await settingService.GetMaximumPlayerFromRoomAsync(applicationInstanceId, roomId);
        }
        public async Task<string> GetNextAvailableRoleAsync(string applicationInstanceId, string roomId, string userId, CancellationToken cancellationToken)
        {
            return await settingService.GetNextAvailableRoleAsync(applicationInstanceId, roomId, userId, cancellationToken);
        }
        public async Task<string> RemoveAssignedRoleAsync(string applicationInstanceId, string roomId, string userId)
        {
            return await settingService.RemoveAssignedRoleAsync(applicationInstanceId, roomId, userId);
        }
        public async Task<List<object>> GetTurnsAsync(string applicationInstanceId, string roomId, string phase, float round)
        {
            return await roleStatusService.GetTurnsAsync(applicationInstanceId, roomId, phase, round);
        }
        public async Task<object> ExtraPlayerInfo(string applicationInstanceId, string roomId, string userId)
        {
            // Fetch the player information from the database
            var player = await dbContext.RoleStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(rs =>
                    rs.ApplicationInstanceId == applicationInstanceId &&
                    rs.RoomId == roomId &&
                    rs.UserId == userId);
            if (player is null)
                throw new Exception($"Player with UserId {userId} not found in room {roomId} for application {applicationInstanceId}.");

            var (token, expireTime) = await azureService.GetTokenAsync(player.ACSUserId); // Ensure the token is fetched before mapping

            // Map to ExternalPlayerInfoModel
            var externalPlayerInfo = new ExternalPlayerInfoModel
            {
                ACSUserId = player.ACSUserId,
                ACSToken = token,
                ACSTokenExpire = expireTime,
            };
            return externalPlayerInfo;
        }
        public async Task<bool> DoActionAsync(string applicationInstanceId, string roomId, string userId, string abilityName, string targetUserId, float round, string phase)
        {
            await gameActionHistoryService.InsertAsync(applicationInstanceId, roomId, userId, abilityName, targetUserId, round, phase);
            return true;
        }
        public async Task<bool> PrepareAfterPhaseAsync(string applicationInstanceId, string roomId, string phase, string preparePhase)
        {
            var phaseService = phaseServiceFactory.GetPhaseService(preparePhase);
            var preparedPhase = await phaseService.Prepared(applicationInstanceId, roomId, phase);
            return true;
        }
    }
}