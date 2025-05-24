using Grpc.Core;
using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server.Data;

namespace Ora.GameManaging.Server.Infrastructure.Services
{
    public interface IGameRoomServices
    {
        Task<GameRoomEntity?> GetRoomByIdAsync(string roomId, CancellationToken cancellationToken = default);
        Task<bool> CreateRoomAsync(GameRoomEntity room, CancellationToken cancellationToken = default);
        Task<bool> UpdateRoomAsync(GameRoomEntity room, CancellationToken cancellationToken = default);
        Task<bool> DeleteRoomAsync(string roomId, CancellationToken cancellationToken = default);
        Task<GetAllRoomsReply> GetAllRooms(GetAllRoomsRequest request,ServerCallContext context);
        Task<GetAllRoomsReply> GetRoomsByAppId(GetRoomByAppIdRequest request, ServerCallContext context);
    }
}