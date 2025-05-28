namespace Ora.GameManaging.Server.Models.Adapter
{
    public class MaxRoomPlayerModel : AdapterModel
    {
        public override string ActionName => "GetMaximumPlayerFromRoomAsync";
        public override string TypeName => "SettingService";

        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
    }
}
