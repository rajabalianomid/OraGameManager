namespace Ora.GameManaging.Server.Models.Adapter
{
    public class MaxRoomPlayerModel : AdapterModel
    {
        public override string ActionName => "GetMaximumPlayerFromRoomAsync";
        public override string TypeName => "GameEngine";

        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
    }
}
