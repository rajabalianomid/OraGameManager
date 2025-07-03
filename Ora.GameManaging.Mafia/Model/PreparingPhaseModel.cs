namespace Ora.GameManaging.Mafia.Model
{
    public class PreparingPhaseModel
    {
        public bool HasVideo { get; set; }
        public List<RoleStatusModel> ActingOn { get; set; } = [];
    }
}
