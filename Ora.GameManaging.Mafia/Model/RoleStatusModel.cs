namespace Ora.GameManaging.Mafia.Model
{
    public class RoleStatusModel
    {
        public required string RoleName { get; set; }
        public int Health { get; set; }
        public int AbilityCount { get; set; }
        public int SelfAbilityCount { get; set; }
        public bool HasNightAbility { get; set; }
        public bool HasDayAbility { get; set; }
        public bool CanSpeak { get; set; }
        public bool DarkSide { get; set; }
        public bool Challenge { get; set; }
        public required string Abilities { get; set; }
    }
}
