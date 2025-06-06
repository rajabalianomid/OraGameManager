using System.Collections.Generic;

namespace Ora.GameManaging.Mafia.Model
{
    public class PhaseModel
    {
        public List<ActionHistoryModel>? Actions { get; set; }
        public List<RoleStatusModel>? RoleStatuses { get; set; }
        public List<AbilityModel>? Abilities { get; set; }
        public List<string> Result { get; set; } = default!;
    }
}
