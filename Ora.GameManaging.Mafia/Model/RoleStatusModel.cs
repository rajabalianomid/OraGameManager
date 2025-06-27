using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model.Mapping;

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
        public List<AbilityModel> Abilities { get; set; } = [];

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
            Abilities = entity.RoleStatusesAbilities.ToAbilityModels();
        }

        public void MakeEmpty()
        {
            RoleName = string.Empty;
            Health = 0;
            AbilityCount = 0;
            SelfAbilityCount = 0;
            HasNightAbility = false;
            HasDayAbility = false;
            CanSpeak = false;
            DarkSide = false;
            Challenge = false;
            Abilities = [];
        }
    }
}
