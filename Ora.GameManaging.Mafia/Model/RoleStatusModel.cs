using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Model
{
    public class RoleStatusModel
    {
        public string RoleName { get; set; } = default!;
        public int Health { get; set; }
        public int AbilityCount { get; set; }
        public int SelfAbilityCount { get; set; }
        public bool HasNightAbility { get; set; }
        public bool HasDayAbility { get; set; }
        public bool CanSpeak { get; set; }
        public bool DarkSide { get; set; }
        public bool Challenge { get; set; }
        public string Abilities { get; set; } = default!;

        public RoleStatusModel() { }
        public RoleStatusModel(RoleStatusEntity entity)
        {
            RoleName = entity.RoleName;
            Health = entity.Health;
            AbilityCount = entity.AbilityCount;
            SelfAbilityCount = entity.SelfAbilityCount;
            HasNightAbility = entity.HasNightAbility;
            HasDayAbility = entity.HasDayAbility;
            CanSpeak = entity.CanSpeak;
            DarkSide = entity.DarkSide;
            Challenge = entity.Challenge;
            Abilities = entity.Abilities;
        }
    }
}
