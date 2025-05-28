using Grpc.Core;
using Grpc.Net.ClientFactory;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Repositories;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;
using Ora.GameManaging.Mafia.Protos;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class SettingService(GeneralAttributeRepository generalAttributeRepository)
    {
        public async Task<string> GetNextAvailableRoleAsync(string applicationInstanceId, string roomId, string userId, CancellationToken cancellationToken)
        {
            // 1. Fetch all attributes for the room
            var attributes = (await generalAttributeRepository.GetByEntityAsync(applicationInstanceId, EntityKeys.GameRoom, roomId))
                .ToList();

            // 2. Check if this user already has an assigned role
            var userRoleKey = $"AssignedRole:{userId}";
            var userRoleAttribute = attributes.FirstOrDefault(a => a.Key == userRoleKey);
            if (userRoleAttribute != null && !string.IsNullOrWhiteSpace(userRoleAttribute.Value))
            {
                // Already assigned, return the same role
                return userRoleAttribute.Value;
            }

            // 3. Get the available roles for this room (comma-separated, e.g. "Citizen,Citizen,Doctor")
            var availableRolesAttribute = attributes.FirstOrDefault(a => a.Key == "AvailableRoles");
            if (availableRolesAttribute == null || string.IsNullOrWhiteSpace(availableRolesAttribute.Value))
                return string.Empty;

            var availableRoles = availableRolesAttribute.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            // 4. Get all assigned roles for this room (from all users)
            var assignedRoles = attributes
                .Where(a => a.Key.StartsWith("AssignedRole:") && !string.IsNullOrWhiteSpace(a.Value))
                .Select(a => a.Value)
                .ToList();

            // 5. Find a role that is still available (count in assigned < count in available)
            foreach (var role in availableRoles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var availableCount = availableRoles.Count(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
                var assignedCount = assignedRoles.Count(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
                if (assignedCount < availableCount)
                {
                    // Assign this role to this user in DB
                    var newUserRole = new GeneralAttributeEntity
                    {
                        ApplicationInstanceId = applicationInstanceId,
                        EntityName = EntityKeys.GameRoom,
                        EntityId = roomId,
                        Key = userRoleKey,
                        Value = role
                    };
                    await generalAttributeRepository.AddAsync(newUserRole);
                    return role;
                }
            }

            // No available roles left
            return string.Empty;
        }

        public async Task<string> RemoveAssignedRoleAsync(string applicationInstanceId, string roomId, string userId)
        {
            var userRoleKey = $"AssignedRole:{userId}";
            var attr = await generalAttributeRepository.GetByEntityAsync(applicationInstanceId, EntityKeys.GameRoom, roomId, userRoleKey);
            if (attr != null)
                await generalAttributeRepository.DeleteAsync(attr.Id);

            return userRoleKey;
        }

        public async Task<int> GetMaximumPlayerFromRoomAsync(string applicationInstanceId, string roomId)
        {
            // Fetch the room attributes
            var attributes = await generalAttributeRepository.GetByEntityAsync(applicationInstanceId, EntityKeys.GameRoom, roomId);
            var maxPlayerAttr = attributes.FirstOrDefault(a => a.Key == SettingKeys.MaxPlayer);

            if (maxPlayerAttr != null && int.TryParse(maxPlayerAttr.Value, out int maxPlayer))
            {
                return maxPlayer;
            }
            return 0;
        }
    }
}
