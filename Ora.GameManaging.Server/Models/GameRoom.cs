using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ora.GameManaging.Server.Models
{
    internal class GameRoom(string appId, string roomId)
    {
        public string AppId { get; set; } = appId;
        public string RoomId { get; set; } = roomId;
        public ConcurrentDictionary<string, PlayerInfo> Players { get; set; } = new();
        public string? CurrentTurnPlayerId { get; set; }
        public int TurnDurationSeconds { get; set; } = 30;
    }
}
