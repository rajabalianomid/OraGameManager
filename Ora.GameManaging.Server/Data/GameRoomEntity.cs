﻿namespace Ora.GameManaging.Server.Data
{
    public class GameRoomEntity
    {
        public int Id { get; set; }
        public string AppId { get; set; } = ""; // FK
        public string RoomId { get; set; } = "";
        public int TurnDurationSeconds { get; set; }
        public string? CurrentTurnPlayers { get; set; } // For group turns, store as comma-separated or JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? LastSnapshotJson { get; set; } // For storing snapshot of game state
        public ICollection<PlayerEntity> Players { get; set; } = new List<PlayerEntity>();
        public ICollection<GameEventEntity> Events { get; set; } = new List<GameEventEntity>();
        public AppInstanceEntity AppInstance { get; set; } = null!;
    }

}
