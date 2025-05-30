namespace Ora.GameManaging.Mafia.Model
{
    public class LatestInformationRequestModel
    {
        public string ApplicationInstanceId { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        public List<PlayerInfoModel> Players { get; set; } = [];
        public string CurrentUser { get; set; } = string.Empty;
        public int TurnDurationSeconds { get; set; } = 0;
    }
}
