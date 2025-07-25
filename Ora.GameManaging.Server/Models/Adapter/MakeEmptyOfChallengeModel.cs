﻿namespace Ora.GameManaging.Server.Models.Adapter
{
    public class MakeEmptyOfChallengeModel : AdapterModel
    {
        public override string ActionName => "MakeEmptyOfChallengeAsync";
        public override string TypeName => "GameEngine";

        public required string AppId { get; set; }
        public required string RoomId { get; set; }
    }
}
