namespace Ora.GameManaging.Server.Models.Adapter
{
    public class TurnModel : AdapterModel
    {
        public override string ActionName => "GetTurnsAsync";
        public override string TypeName => "GameEngine";

        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
        public required string Phase { get; set; }
        public required float Round { get; set; }
    }
}
