using Grpc.Core;
using Grpc.Net.ClientFactory;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Repositories;
using Ora.GameManaging.Mafia.Model.Mapping;
using Ora.GameManaging.Mafia.Protos;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class SettingService(GeneralAttributeRepository generalAttributeRepository)
    {
        public async Task<GameNextRoleReply> GetNextAvailableRoleAsync(string applicationInstanceId, string roomId, CancellationToken cancellationToken)
        {
            // 1. Fetch all attributes for the room
            var attributes = (await generalAttributeRepository.GetByEntityAsync(applicationInstanceId, EntityKeys.GameRoom, roomId))
                .ToList();

            // 2. Get the available roles for this room (comma-separated, e.g. "Citizen,Citizen,Doctor")
            var availableRolesAttribute = attributes.FirstOrDefault(a => a.Key == "AvailableRoles");
            if (availableRolesAttribute == null || string.IsNullOrWhiteSpace(availableRolesAttribute.Value))
                return new GameNextRoleReply();

            var availableRoles = availableRolesAttribute.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            // 3. Get the assigned roles for this room (comma-separated)
            var assignedRolesAttribute = attributes.FirstOrDefault(a => a.Key == "AssignedRoles");
            var assignedRoles = new List<string>();
            if (assignedRolesAttribute != null && !string.IsNullOrWhiteSpace(assignedRolesAttribute.Value))
            {
                assignedRoles = [.. assignedRolesAttribute.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
            }

            // 4. Find a role that is still available (count in assigned < count in available)
            foreach (var role in availableRoles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var availableCount = availableRoles.Count(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
                var assignedCount = assignedRoles.Count(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
                if (assignedCount < availableCount)
                {
                    // Assign this role
                    assignedRoles.Add(role);
                    var updatedAssignedRoles = string.Join(",", assignedRoles);

                    if (assignedRolesAttribute != null)
                    {
                        assignedRolesAttribute.Value = updatedAssignedRoles;
                        await generalAttributeRepository.UpdateAsync(assignedRolesAttribute);
                    }
                    else
                    {
                        await generalAttributeRepository.AddAsync(new GeneralAttributeEntity
                        {
                            ApplicationInstanceId = applicationInstanceId,
                            EntityName = EntityKeys.GameRoom,
                            EntityId = roomId,
                            Key = "AssignedRoles",
                            Value = updatedAssignedRoles
                        });
                    }
                    return new GameNextRoleReply { Role = role };
                }
            }

            // No available roles left
            return new GameNextRoleReply();
        }
    }
}
