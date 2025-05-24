using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Model.Mapping;
using System.Threading;

namespace Ora.GameManaging.Server.Infrastructure.Services
{
    public class GameRoomServices(RoomRepository repository) : GameRoomGrpc.GameRoomGrpcBase, IGameRoomServices
    {
        public Task<bool> CreateRoomAsync(GameRoomEntity room, CancellationToken cancellationToken = default)
            => repository.AddAsync(room, cancellationToken);

        public Task<bool> DeleteRoomAsync(string roomId, CancellationToken cancellationToken = default)
            => repository.DeleteAsync(roomId, cancellationToken);

        public Task<GameRoomEntity?> GetRoomByIdAsync(string roomId, CancellationToken cancellationToken = default)
            => repository.GetByIdAsync(roomId, cancellationToken);

        public Task<bool> UpdateRoomAsync(GameRoomEntity room, CancellationToken cancellationToken = default)
            => repository.UpdateAsync(room, cancellationToken);

        public override async Task<GetAllRoomsReply> GetAllRooms(GetAllRoomsRequest request, ServerCallContext context)
        {
            var rooms = await repository.GetAllAsync(context.CancellationToken);
            var reply = new GetAllRoomsReply();
            reply.Rooms.AddRange(rooms.Select(room => room.ToModel()));
            return reply;
        }

        public override async Task<GetAllRoomsReply> GetRoomsByAppId(GetRoomByAppIdRequest request, ServerCallContext context)
        {
            var rooms = await repository.GetByAppIdAsync(request.AppId, context.CancellationToken);
            var reply = new GetAllRoomsReply();
            reply.Rooms.AddRange(rooms.Select(room => room.ToModel()));
            return reply;
        }
    }
}
