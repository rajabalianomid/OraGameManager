﻿using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Migrations;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;
using System.Collections.Concurrent;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class SettingService(MafiaDbContext dbContext, AzureService azureService)
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public async Task<string> GetNextAvailableRoleAsync(string applicationInstanceId, string roomId, string userId, CancellationToken cancellationToken)
        {
            var key = $"{applicationInstanceId}:{roomId}";
            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));


            await semaphore.WaitAsync(cancellationToken);

            try
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
                foreach (var role in availableRoles.Except(assignedRoles).Distinct(StringComparer.OrdinalIgnoreCase))
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
                        var foundAssignedRole = await dbContext.RoleStatuses.FirstOrDefaultAsync(w => w.UserId == userId && w.RoomId == roomId && w.ApplicationInstanceId == applicationInstanceId);

                        if (foundAssignedRole == null)
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
                            //await dbContext.SaveChangesAsync(cancellationToken);

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

                            var acsUserId = await azureService.CreateUserAsync();

                            var roleAbility = roleAttributes.Where(w => w.Key == "Abilities").Select(s => s.Value).FirstOrDefault();
                            var settingAbilities = roleAbility?.Split(';').ToList();

                            List<AbilityEntity> abilities = [];

                            if (settingAbilities != null)
                                abilities = [.. dbContext.AbilityEntities.Where(w => settingAbilities.Contains(w.Name))];

                            var roleStatus = new RoleStatusEntity
                            {
                                ApplicationInstanceId = applicationInstanceId,
                                RoomId = roomId,
                                UserId = userId,
                                RoleName = role,
                                Turn = maxTurn + 2, // Increment turn for the new role
                                ACSUserId = acsUserId,
                                LastUpdated = DateTime.UtcNow,
                                RoleStatusesAbilities = [.. abilities.Select(a => new RoleStatusesAbilityEntity
                            {
                                AbilityId = a.Id,
                            })],
                            };

                            AttributeReflectionHelper.ApplyAttributesToModel(roleStatus, roleAttributes);


                            dbContext.RoleStatuses.Add(roleStatus);

                            await dbContext.SaveChangesAsync(cancellationToken);
                        }
                        else
                        {
                            return foundAssignedRole.RoleName;
                        }

                        return role;
                    }
                }
            }
            finally
            {
                semaphore.Release();
                if (semaphore.CurrentCount == 1)
                    _locks.TryRemove(key, out _);
            }
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
