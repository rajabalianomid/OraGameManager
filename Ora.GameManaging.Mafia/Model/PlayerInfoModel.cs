namespace Ora.GameManaging.Mafia.Model
{
    public class PlayerInfoModel
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Health { get; set; }
        public bool IsAlive => Health > 0;
    }
}