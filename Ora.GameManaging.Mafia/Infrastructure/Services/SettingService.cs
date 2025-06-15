using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class SettingService(MafiaDbContext dbContext, AzureService azureService)
    {
        public async Task<string> GetNextAvailableRoleAsync(string applicationInstanceId, string roomId, string userId, CancellationToken cancellationToken)
        {
            // 1. Fetch all attributes for the room
            var attributes = await dbContext.GeneralAttributes
                .Where(a => a.ApplicationInstanceId == applicationInstanceId && a.EntityName == EntityKeys.GameRoom && a.EntityId == roomId)
                .ToListAsync(cancellationToken);

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

            var roomRoleSettings = (await dbContext.GeneralAttributes
                        .Where(a => a.EntityName == EntityKeys.RoomRole && a.EntityId == roomId)
                        .ToListAsync());
            foreach (var role in availableRoles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var maxTurn = dbContext.RoleStatuses
                        .Where(rs => rs.ApplicationInstanceId == applicationInstanceId && rs.RoomId == roomId)
                        .Select(rs => (int?)rs.Turn)
                        .OrderByDescending(o => o)
                        .FirstOrDefault() ?? 0;
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
                    dbContext.GeneralAttributes.Add(newUserRole);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Populate RoleStatusEntity from GeneralAttributeEntity
                    var prefix = $"{role}_";
                    var roleAttributes = roomRoleSettings.Where(w => w.Key.StartsWith(prefix))
                        .Select(a => new GeneralAttributeEntity
                        {
                            ApplicationInstanceId = a.ApplicationInstanceId,
                            EntityName = a.EntityName,
                            EntityId = a.EntityId,
                            Key = a.Key[(role.Length + 1)..], // Remove role prefix and underscore
                            Value = a.Value
                        }).ToList();



                    if (!dbContext.RoleStatuses.Any(w => w.UserId == userId && w.RoomId == roomId && w.ApplicationInstanceId == applicationInstanceId))
                    {
                        var acsUserId = await azureService.CreateUserAsync();

                        var roleStatus = new RoleStatusEntity
                        {
                            ApplicationInstanceId = applicationInstanceId,
                            RoomId = roomId,
                            UserId = userId,
                            RoleName = role,
                            Abilities = string.Empty, // Initialize with empty abilities
                            Turn = maxTurn + 2, // Increment turn for the new role
                            ACSUserId = acsUserId,
                            LastUpdated = DateTime.UtcNow
                        };

                        AttributeReflectionHelper.ApplyAttributesToModel(roleStatus, roleAttributes);
                        dbContext.RoleStatuses.Add(roleStatus);
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }

                    return role;
                }
            }

            // No available roles left
            return string.Empty;
        }

        public async Task<string> RemoveAssignedRoleAsync(string applicationInstanceId, string roomId, string userId)
        {
            var userRoleKey = $"AssignedRole:{userId}";
            var attr = await dbContext.GeneralAttributes
                .FirstOrDefaultAsync(a =>
                    a.ApplicationInstanceId == applicationInstanceId &&
                    a.EntityName == EntityKeys.GameRoom &&
                    a.EntityId == roomId &&
                    a.Key == userRoleKey);
            if (attr != null)
            {
                dbContext.GeneralAttributes.Remove(attr);
                await dbContext.SaveChangesAsync();
            }

            return userRoleKey;
        }

        public async Task<int> GetMaximumPlayerFromRoomAsync(string applicationInstanceId, string roomId)
        {
            // Fetch the room attributes
            var attributes = await dbContext.GeneralAttributes
               .Where(a => a.ApplicationInstanceId == applicationInstanceId && a.EntityName == EntityKeys.GameRoom && a.EntityId == roomId)
               .ToListAsync();

            var maxPlayerAttr = attributes.FirstOrDefault(a => a.Key == SettingKeys.MaxPlayer);

            if (maxPlayerAttr != null && int.TryParse(maxPlayerAttr.Value, out int maxPlayer))
            {
                return maxPlayer;
            }
            return 0;
        }
    }
}
