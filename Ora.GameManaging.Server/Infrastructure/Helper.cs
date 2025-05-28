using Ora.GameManaging.Server.Models;
using System.Text.Json;

namespace Ora.GameManaging.Server.Infrastructure
{
    public static class Helper
    {
        /// <summary>
        /// Serializes the GameRoom object using GameHub's SerializeRoom method.
        /// </summary>
        public static string ToJsonSerialize<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static PlayerInfo? GetPlayerInfo(this GameRoom room, string playerId)
        {
            if (room.Players.TryGetValue(playerId, out var playerInfo))
            {
                return playerInfo;
            }
            return null;
        }
    }
}
