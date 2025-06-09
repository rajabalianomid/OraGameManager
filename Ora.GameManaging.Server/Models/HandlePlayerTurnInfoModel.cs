namespace Ora.GameManaging.Server.Models
{
    public class HandlePlayerTurnInfoModel
    {
        public bool IsAlive { get; set; }
        public bool Success { get; set; }
        public List<string> Messages { get; set; } = [];
    }
}
