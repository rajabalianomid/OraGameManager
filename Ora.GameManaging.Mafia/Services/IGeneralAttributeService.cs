using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Services
{
    public interface IGeneralAttributeService
    {
        Task<GeneralAttributeEntity?> GetAsync(int id);
        Task<IEnumerable<GeneralAttributeEntity>> GetByEntityAsync(string applicationInstanceId, string entityName, string entityId);
        Task<IEnumerable<GeneralAttributeEntity>> GetAllByApplicationIdAsync(string applicationInstanceId, string entityName);
        Task AddAsync(GeneralAttributeEntity attribute);
        Task UpdateAsync(GeneralAttributeEntity attribute);
        Task DeleteAsync(int id);
    }
}