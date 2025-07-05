namespace Ora.GameManaging.Mafia.Model
{
    public class PreparingPhaseModel
    {
        public bool HasVideo { get; set; }
        public List<string> ActingOn { get; set; } = [];
        public string Information { get; set; } = string.Empty;
    }
}
