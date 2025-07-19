namespace Ora.GameManaging.Server.Models.Adapter
{
    public class ActionHistoryCommitModel : AdapterModel
    {
        public override string ActionName => "DoActionCommitAsync";
        public override string TypeName => "GameEngine";

        public required string ApplicationInstanceId { get; set; }
        public required string RoomId { get; set; }
        public required string UserId { get; set; }
        public required string AbilityName { get; set; }
        public required string TargetUserId { get; set; }
        public required float Round { get; set; }
        public required string Phase { get; set; }
    }
}
