using Grpc.Core;
using Ora.GameManaging.Mafia.Protos;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public interface ISettingService
    {
        Task<GameNextRoleReply> GetNextAvailableRole(GetSettingRoomByIdRequest request, ServerCallContext context);
    }
}