using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Repositories
{
    public class GeneralAttributeRepository(MafiaDbContext context)
    {
        public async Task<GeneralAttributeEntity?> GetAsync(int id) =>
            await context.GeneralAttributes.FindAsync(id);

        public async Task<IEnumerable<GeneralAttributeEntity>> GetByEntityAsync(string applicationInstanceId, string entityName, string entityId) =>
            await context.GeneralAttributes
                .Where(a => a.ApplicationInstanceId == applicationInstanceId && a.EntityName == entityName && a.EntityId == entityId)
                .ToListAsync();

        public async Task<IEnumerable<GeneralAttributeEntity>> GetEntitiesAsync(string applicationInstanceId, string entityName) =>
            await context.GeneralAttributes
                .Where(a => a.ApplicationInstanceId == applicationInstanceId && a.EntityName == entityName)
                .ToListAsync();

        public async Task AddAsync(GeneralAttributeEntity attribute)
        {
            context.GeneralAttributes.Add(attribute);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GeneralAttributeEntity attribute)
        {
            context.GeneralAttributes.Update(attribute);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await context.GeneralAttributes.FindAsync(id);
            if (entity != null)
            {
                context.GeneralAttributes.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}