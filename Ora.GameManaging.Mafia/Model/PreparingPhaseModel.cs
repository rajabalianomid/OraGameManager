namespace Ora.GameManaging.Mafia.Model
{
    public class PreparingPhaseModel
    {
        public bool HasVideo { get; set; }
        public List<string> ActingOn { get; set; } = [];
        public List<LastCardChanceModel> Cards { get; set; } = [];
        public List<string> Roles { get; set; } = [];
        public BasePlayerInfo SelectedPlayer { get; set; } = new();
        public bool ForceAction { get; set; }
        public string Information { get; set; } = string.Empty;
        public string SelectedAbility { get; set; } = string.Empty;
    }
}
