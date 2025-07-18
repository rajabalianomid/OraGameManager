namespace Ora.GameManaging.Mafia.Model
{
    public class LastCardChanceModel
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public bool SelfAct { get; set; }
    }
}