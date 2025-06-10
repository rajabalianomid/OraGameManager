namespace Ora.GameManaging.Server.Models
{
    public class NextPhaseResponseModel
    {
        public bool IsLastPhase { get; set; }
        public string Name { get; set; } = default!;
    }
}