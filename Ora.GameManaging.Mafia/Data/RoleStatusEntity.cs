using System.Collections.Generic;

namespace Ora.GameManaging.Mafia.Data
{
    public class RoleStatusEntity
    {
        public int Id { get; set; }
        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
        public required string UserId { get; set; }
        public required string RoleName { get; set; }
        public int Health { get; set; }
        public int AbilityCount { get; set; }
        public int SelfAbilityCount { get; set; }
        public bool HasNightAbility { get; set; }
        public bool HasDayAbility { get; set; }
        public bool CanSpeak { get; set; }
        public bool DarkSide { get; set; }
        public required string Abilities { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}