using Ora.GameManaging.Server.Models;
using System.Text.Json;

namespace Ora.GameManaging.Server.Infrastructure
{
    public static class Helper
    {
        /// <summary>
        /// Serializes the object.
        /// </summary>
        public static string ToJsonSerialize<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        public static T? ToJsonDeserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json);
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
