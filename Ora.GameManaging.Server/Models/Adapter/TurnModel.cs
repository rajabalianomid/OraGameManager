namespace Ora.GameManaging.Server.Models.Adapter
{
    public class TurnModel : AdapterModel
    {
        public override string ActionName => "GetTurnsAsync";
        public override string TypeName => "RoleStatusService";

        public string ApplicationInstanceId { get; set; } = "";
        public string RoomId { get; set; } = "";
    }
}
