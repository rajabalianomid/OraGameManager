namespace Ora.GameManaging.Mafia.Data
{
    public class GameActionHistoryEntity
    {
        public long Id { get; set; }
        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
        public required string ActorUserId { get; set; }
        public required string ActorRole { get; set; }
        public required string TargetUserId { get; set; }
        public required int ActionTime { get; set; }
        public required float Round { get; set; } = 1; // Default to 1 for the first action, 1.5 means night of day 1, etc.
        public bool IsProcessed { get; set; }
        public string? Result { get; set; }

        public int AbilityId { get; set; }
        public required AbilityEntity Ability { get; set; }
    }
}