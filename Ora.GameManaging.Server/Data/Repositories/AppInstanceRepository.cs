using Microsoft.EntityFrameworkCore;

namespace Ora.GameManaging.Server.Data.Repositories
{
    public class AppInstanceRepository(GameDbContext db)
    {
        public async Task<AppInstanceEntity> AddAsync(string appId, string name, string? owner, string? description)
        {
            var app = new AppInstanceEntity
            {
                AppId = appId,
                Name = name,
                Owner = owner,
                Description = description
            };
            db.AppInstances.Add(app);
            await db.SaveChangesAsync();
            return app;
        }

        public async Task<AppInstanceEntity?> GetAsync(string appId)
        {
            return await db.AppInstances.Include(a => a.Rooms).FirstOrDefaultAsync(a => a.AppId == appId);
        }

        public async Task<List<AppInstanceEntity>> GetAllAsync()
        {
            return await db.AppInstances.ToListAsync();
        }
    }
}
