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
        public int Turn { get; set; }
        public bool Challenge { get; set; }
        public string ACSUserId { get; set; } = default!;
        public int VoteCount { get; set; } = 0;
        public int TempVoteCount { get; set; } = 0;
        public bool Selected { get; set; }
        public bool ActingOnMe { get; set; }
        public bool Lock { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public required ICollection<RoleStatusesAbilityEntity> RoleStatusesAbilities { get; set; }
    }
}