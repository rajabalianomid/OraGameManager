using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Model
{
    public class ActionHistoryModel
    {
        public long Id { get; set; }
        public string ApplicationInstanceId { get; set; } = default!;
        public string RoomId { get; set; } = default!;
        public string ActorUserId { get; set; } = default!;
        public string ActorRole { get; set; } = default!;
        public string TargetUserId { get; set; } = default!;
        public DateTime ActionTime { get; set; }
        public float Round { get; set; }
        public bool IsProcessed { get; set; }
        public string? Result { get; set; }
        public int AbilityId { get; set; }
        public AbilityModel? Ability { get; set; }

        public ActionHistoryModel() { }

        public ActionHistoryModel(GameActionHistoryEntity entity)
        {
            Id = entity.Id;
            ApplicationInstanceId = entity.ApplicationInstanceId;
            RoomId = entity.RoomId;
            ActorUserId = entity.ActorUserId;
            ActorRole = entity.ActorRole;
            TargetUserId = entity.TargetUserId;
            //ActionTime = entity.ActionTime;
            Round = entity.Round;
            IsProcessed = entity.IsProcessed;
            Result = entity.Result;
            AbilityId = entity.AbilityId;
            Ability = entity.Ability != null ? new AbilityModel(entity.Ability) : null;
        }
    }
}   