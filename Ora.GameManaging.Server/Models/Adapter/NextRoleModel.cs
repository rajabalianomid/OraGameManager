namespace Ora.GameManaging.Server.Models.Adapter
{
    public class NextRoleModel : AdapterModel
    {
        public override string ActionName => "GetNextAvailableRoleAsync";
        public override string TypeName => "GameEngine";

        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
        public required string UserId { get; set; }
    }
}
