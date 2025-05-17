namespace Ora.GameManaging.Server.Data
{
    public class GameEventEntity
    {
        public int Id { get; set; }
        public int GameRoomEntityId { get; set; }
        public string? PlayerName { get; set; }
        public string EventType { get; set; } = "";
        public string? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public GameRoomEntity GameRoom { get; set; } = null!;
    }
}
