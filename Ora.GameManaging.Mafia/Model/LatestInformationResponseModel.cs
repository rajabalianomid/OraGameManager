
namespace Ora.GameManaging.Mafia.Model
{
    public class LatestInformationResponseModel
    {
        public LatestInformationDataResponseModel Data { get; set; } = default!;
        public ExtraInfoDetailsModel ExtraInfo { get; set; } = default!;
    }
    public class LatestInformationDataResponseModel
    {
        public string? Phase { get; internal set; }
        public int Round { get; internal set; }
        public bool CanSpeak { get; internal set; }
        public List<string>? Abilities { get; internal set; }
        public RoleStatusModel? RoleStatus { get; internal set; }
        public List<PlayerInfoModel> AlivePlayers { get; set; } = [];
        public List<PlayerInfoModel> DeadPlayers { get; set; } = [];
    }
}
