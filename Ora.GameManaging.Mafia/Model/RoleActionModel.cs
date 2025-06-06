using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Model
{
    public class RoleActionModel
    {
        public required string RoleName { get; set; }
        public required string Expression { get; set; }
    }
    public class ScriptGlobals
    {
        public required List<GameActionHistoryEntity> Actions { get; set; }
        public required List<RoleStatusEntity> RoleStatuses { get; set; }
        public required List<AbilityEntity> Abilities { get; set; }
    }
}
