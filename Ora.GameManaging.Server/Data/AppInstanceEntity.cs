namespace Ora.GameManaging.Server.Data
{
    public class AppInstanceEntity
    {
        public string AppId { get; set; } = Guid.NewGuid().ToString(); // Primary Key
        public string Name { get; set; } = "";
        public string? Owner { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GameRoomEntity> Rooms { get; set; } = new List<GameRoomEntity>();
    }
}
