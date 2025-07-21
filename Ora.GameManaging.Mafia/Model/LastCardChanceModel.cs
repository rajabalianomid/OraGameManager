namespace Ora.GameManaging.Mafia.Model
{
    public class LastCardChanceModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public bool SelfAct { get; set; }
        public bool ShowFront { get; set; }
    }
}