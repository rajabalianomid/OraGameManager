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
        public string? CurrentTurnPlayerId { get; set; }
        public int TurnDurationSeconds { get; set; } = 10;

        public string Serialize()
        {
            return new
            {
                AppId,
                RoomId,
                Players = Players.Values.Select(p => new { p.ConnectionId, p.UserId, p.Name, p.Role, p.Status }),
                TurnDurationSeconds
            }.ToJsonSerialize();
        }
    }
}
