
namespace Ora.GameManaging.Mafia.Model
{
    public class LatestInformationResponseModel
    {
        public LatestInformationDataResponseModel Data { get; set; } = default!;
        public ExtraInfoDetailsModel ExtraInfo { get; set; } = default!;
    }
    public class LatestInformationDataResponseModel
    {
        public string UserId { get; set; } = default!;
        public string? Phase { get; internal set; }
        public int Round { get; internal set; }

        private RoleStatusModel? _roleStatus;
        public RoleStatusModel? RoleStatus
        {
            get => _roleStatus;
            internal set
            {
                _roleStatus = value;
                IsAlive = _roleStatus?.Health > 0;
            }
        }
        public bool IsAlive { get; private set; }
        public List<AbilityModel> Abilities { get; set; } = [];
        public List<BasePlayerInfo> AlivePlayers { get; set; } = [];
        public List<BasePlayerInfo> DeadPlayers { get; set; } = [];
        public List<BasePlayerInfo> ActingOn { get; set; } = [];
        public bool HasVideo { get; set; }
    }
}
