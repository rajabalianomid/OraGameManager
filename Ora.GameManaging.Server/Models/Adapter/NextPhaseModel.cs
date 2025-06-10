namespace Ora.GameManaging.Server.Models.Adapter
{
    public class NextPhaseModel : AdapterModel
    {
        public override string ActionName => "GetNextPhase";
        public override string TypeName => "GameEngine";

        public required string currentPhase { get; set; }
    }
}
