namespace Ora.GameManaging.Server.Data
{
    public class PlayerEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";   // Unique per user
        public string ConnectionId { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Role { get; set; }
        public int Status { get; set; }
        public int GameRoomEntityId { get; set; }
        public DateTime LastSeen { get; set; }
        public GameRoomEntity GameRoom { get; set; } = null!;
    }
}
