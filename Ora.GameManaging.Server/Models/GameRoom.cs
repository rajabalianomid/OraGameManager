using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ora.GameManaging.Server.Infrastructure;

namespace Ora.GameManaging.Server.Models
{
    public class GameRoom(string appId, string roomId)
    {
        public string AppId { get; set; } = appId;
        public string RoomId { get; set; } = roomId;
        public ConcurrentDictionary<string, PlayerInfo> Players { get; set; } = new();
        public ConcurrentBag<string> CurrentTurnPlayersId { get; set; } = [];
        public int TurnDurationSeconds { get; set; } = 10;
        public string Phase { get; set; } = "Lobey";
        public float Round { get; set; }

        public string Serialize(string? targetPlayerId = null, bool isYourTurn = false)
        {
            return new
            {
                AppId,
                RoomId,
                TargetPlayersId = targetPlayerId == null ? CurrentTurnPlayersId : [targetPlayerId],
                Players = Players.Values.Select(p => new { p.ConnectionId, p.UserId, p.Name, p.Role, p.Status, RoomId }),
                TurnDurationSeconds,
                Phase,
                Round,
                IsYourTurn = isYourTurn
            }.ToJsonSerialize();
        }
    }
}
