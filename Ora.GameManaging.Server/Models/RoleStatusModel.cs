namespace Ora.GameManaging.Server.Models
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
        public required string Abilities { get; set; }
    }
}
