using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Model.Mapping
{
    public static class AbilityMappingExtensions
    {
        public static AbilityModel ToAbilityModel(this RoleStatusesAbilityEntity entity)
        {
            return new AbilityModel(entity.Ability);
        }

        public static List<AbilityModel> ToAbilityModels(this IEnumerable<RoleStatusesAbilityEntity> entities)
        {
            if (entities == null)
            {
                return [];
            }
            return [.. entities.Select(e => new AbilityModel(e.Ability))];
        }
    }
}
