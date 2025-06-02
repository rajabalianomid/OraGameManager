namespace Ora.GameManaging.Server.Data
{
    public class GameRoomEntity
    {
        public int Id { get; set; }
        public string AppId { get; set; } = ""; // FK
        public string RoomId { get; set; } = "";
        public string Phase { get; set; } = "Lobby";
        public float Round { get; set; } = 0;
        public bool IsGameStarted { get; set; }
        public int TurnDurationSeconds { get; set; }
        public string? CurrentTurnPlayer { get; set; } // For group turns, store as comma-separated or JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? LastSnapshotJson { get; set; } // For storing snapshot of game state
        public ICollection<PlayerEntity> Players { get; set; } = [];
        public ICollection<GameEventEntity> Events { get; set; } = [];
        public AppInstanceEntity AppInstance { get; set; } = null!;
    }

}
