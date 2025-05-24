using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Repositories;

namespace Ora.GameManaging.Mafia.Services
{
    public class GeneralAttributeService(GeneralAttributeRepository repository) : IGeneralAttributeService
    {
        public Task<GeneralAttributeEntity?> GetAsync(int id) => repository.GetAsync(id);
        public Task<IEnumerable<GeneralAttributeEntity>> GetByEntityAsync(string applicationInstanceId, string entityName, string entityId) =>
            repository.GetByEntityAsync(applicationInstanceId, entityName, entityId);
        public Task<IEnumerable<GeneralAttributeEntity>> GetAllByApplicationIdAsync(string applicationInstanceId, string entityName) =>
            repository.GetEntitiesAsync(applicationInstanceId, entityName);
        public Task AddAsync(GeneralAttributeEntity attribute) => repository.AddAsync(attribute);
        public Task UpdateAsync(GeneralAttributeEntity attribute) => repository.UpdateAsync(attribute);
        public Task DeleteAsync(int id) => repository.DeleteAsync(id);
    }
}