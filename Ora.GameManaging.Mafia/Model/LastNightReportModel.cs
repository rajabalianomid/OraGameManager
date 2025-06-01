namespace Ora.GameManaging.Mafia.Model
{
    public class LastNightReportModel
    {
        public required List<NightEventModel> Events { get; set; }
    }

    public class NightEventModel
    {
        public required string ActionType { get; set; } // e.g., "Kill", "Save", "Investigate"
        public required string ActorUserId { get; set; }
        public required string ActorRole { get; set; }
        public required string TargetUserId { get; set; }
        public required string TargetRole { get; set; }
        public required string Result { get; set; } // e.g., "Success", "Blocked", "Saved"
        public DateTime Timestamp { get; set; }
    }
}
