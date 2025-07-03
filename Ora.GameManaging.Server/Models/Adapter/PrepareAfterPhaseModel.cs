namespace Ora.GameManaging.Server.Models.Adapter
{
    public class PrepareAfterPhaseModel : AdapterModel
    {
        public override string ActionName => "PrepareAfterPhaseAsync";
        public override string TypeName => "GameEngine";

        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
        public required string Phase { get; set; }
        public required string PreparePhase { get; set; }
    }
}
