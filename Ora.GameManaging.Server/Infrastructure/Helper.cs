using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        public static JsonObject? ToJsonNode(this object request)
        {
            var latestinfoJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return JsonNode.Parse(latestinfoJson)?.AsObject();
        }

        public static List<GeneralAttributeEntity> MapToGeneralAttributes(this object source, string entityKey, string entityId)
        {
            var generalAttributes = new List<GeneralAttributeEntity>();

            if (source is string jsonString)
            {
                // Try to parse as JSON
                var jsonObject = JsonNode.Parse(jsonString)?.AsObject();
                if (jsonObject != null)
                {
                    foreach (var prop in jsonObject)
                    {
                        generalAttributes.Add(new GeneralAttributeEntity
                        {
                            Key = prop.Key,
                            Value = prop.Value?.ToString(),
                            EntityKey = entityKey,
                            EntityId = entityId,
                            Description = null
                        });
                    }
                    return generalAttributes;
                }
            }
            else if (source is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in jsonElement.EnumerateObject())
                {
                    generalAttributes.Add(new GeneralAttributeEntity
                    {
                        Key = prop.Name,
                        Value = prop.Value.ToString(),
                        EntityKey = entityKey,
                        EntityId = entityId,
                        Description = null
                    });
                }
                return generalAttributes;
            }

            var type = source.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(source)?.ToString();
                generalAttributes.Add(new GeneralAttributeEntity
                {
                    Key = prop.Name,
                    Value = value,
                    EntityKey = entityKey,
                    EntityId = entityId,
                    Description = null
                });
            }

            return generalAttributes;
        }
    }
}
