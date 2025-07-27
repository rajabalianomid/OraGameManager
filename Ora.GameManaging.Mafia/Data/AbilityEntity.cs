namespace Ora.GameManaging.Mafia.Data
{
    public class AbilityEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; } // Ability name (e.g., "Heal", "Kill")
        public string? Description { get; set; }
        public string? Expression { get; set; } // For dynamic logic, if needed
        public bool IsNightAbility { get; set; }
        public bool IsDayAbility { get; set; }
        public string? RelatedPhase { get; set; }
        public bool IsCard { get; set; }
        public string? Icon { get; set; }
        public bool? SelfAct { get; set; }
        public bool PreparingPhase { get; set; }
        public bool ForceAction { get; set; }
        public int? ParentId { get; set; }
        public required string Pattern { get; set; } = string.Empty;

        public required AbilityEntity Parent { get; set; }
        public required ICollection<AbilityEntity> Children { get; set; }
        public required ICollection<GameActionHistoryEntity> GameActions { get; set; }
        public required ICollection<RoleStatusesAbilityEntity> RoleStatusesAbilities { get; set; }
    }
}