using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Models;
using Ora.GameManaging.Server.Models.Mapping;
using System.Threading;

namespace Ora.GameManaging.Server.Infrastructure.Services
{
    public class GameRoomService(RoomRepository repository, GameRoomRepository gameRoomRepository) : GameRoomGrpc.GameRoomGrpcBase, IGameRoomService
    {
        private readonly RoomRepository repository = repository;
        private readonly GameRoomRepository gameRoomRepository = gameRoomRepository;

        public Task<bool> CreateRoomAsync(GameRoomEntity room, CancellationToken cancellationToken = default)
            => repository.AddAsync(room, cancellationToken);

        public Task<bool> DeleteRoomAsync(string roomId, CancellationToken cancellationToken = default)
            => repository.DeleteAsync(roomId, cancellationToken);

        public Task<GameRoomEntity?> GetRoomByIdAsync(string roomId, CancellationToken cancellationToken = default)
            => repository.GetByIdAsync(roomId, cancellationToken);

        public Task<bool> UpdateRoomAsync(GameRoomEntity room, CancellationToken cancellationToken = default)
            => repository.UpdateAsync(room, cancellationToken);

        public async Task UpdateCurrentTurnAndSyncCacheAsync(string appId, string roomId, List<string> userIds)
        {
            // Update the current turn in the database
            await gameRoomRepository.UpdateCurrentTurnAsync(appId, roomId, string.Join(";", userIds));

            // Retrieve the latest room data from the database
            var updatedRoomEntity = await gameRoomRepository.GetByRoomIdAsync(appId, roomId);
            if (updatedRoomEntity != null)
            {
                var key = $"{appId}:{roomId}";
                GameManager.Rooms[key].CurrentTurnPlayersId = updatedRoomEntity.CurrentTurnPlayer == null ? [] : [.. updatedRoomEntity.CurrentTurnPlayer.Split(';')];
            }
        }

        public override async Task<GetAllRoomsReply> GetAllRooms(GetAllRoomsRequest request, ServerCallContext context)
        {
            var rooms = await repository.GetAllAsync(context.CancellationToken);
            var reply = new GetAllRoomsReply();
            reply.Rooms.AddRange(rooms.Select(room => room.ToModel()));
            return reply;
        }

        public override async Task<GetAllRoomsReply> GetRoomsByAppId(GetRoomByAppIdRequest request, ServerCallContext context)
        {
            var rooms = await repository.GetByAppIdAsync(request.AppId, request.Pagination.Size, request.Pagination.Skip, request.Pagination.Count, context.CancellationToken);
            var reply = new GetAllRoomsReply();
            reply.Rooms.AddRange(rooms.Select(room => room.ToModel()));
            return reply;
        }
    }
}
