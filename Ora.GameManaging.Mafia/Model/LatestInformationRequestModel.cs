namespace Ora.GameManaging.Mafia.Model
{
    public class LatestInformationRequestModel
    {
        public string AppId { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        public List<PlayerInfoModel> Players { get; set; } = [];
        public string CurrentTurnPlayerId { get; set; } = string.Empty;
        public int TurnDurationSeconds { get; set; } = 0;
        public string Phase { get; set; } = string.Empty;
    }
}
