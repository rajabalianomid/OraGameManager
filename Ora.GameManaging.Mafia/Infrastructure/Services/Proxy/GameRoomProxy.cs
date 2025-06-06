﻿using Grpc.Net.ClientFactory;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;
using Ora.GameManaging.Mafia.Protos;
using static Ora.GameManaging.Mafia.Protos.GameRoomGrpc;

namespace Ora.GameManaging.Mafia.Infrastructure.Services.Proxy
{
    public class GameRoomProxy(GrpcClientFactory clientFactory, MafiaDbContext dbContext)
    {
        public GameRoomGrpcClient Instance { get; set; } = clientFactory.CreateClient<GameRoomGrpcClient>("GameManaging");

        public async Task<RoomResultModel> PrepareRoomByAppIdAsync(RoomRequestModel requestModel, CancellationToken cancellationToken)
        {
            var response = await Instance.GetRoomsByAppIdAsync(
                new GetRoomByAppIdRequest
                {
                    AppId = requestModel.AppId,
                    Pagination = new Pagination
                    {
                        Count = requestModel.Count,
                        Size = requestModel.Size,
                        Skip = requestModel.Skip
                    }
                },
                cancellationToken: cancellationToken);

            var rooms = response.Rooms.Select(r => r.ToModel()).ToList();

            var attributes = await dbContext.GeneralAttributes
                .Where(a => a.ApplicationInstanceId == requestModel.AppId && a.EntityName == EntityKeys.GameRoom)
                .ToListAsync(cancellationToken);

            var attributesByRoom = attributes
                .GroupBy(a => a.EntityId)
                .ToDictionary(g => g.Key, g => g.ToList());

            rooms.ForEach(room =>
            {
                if (attributesByRoom.TryGetValue(room.RoomId, out var roomAttributes))
                {
                    AttributeReflectionHelper.ApplyAttributesToModel(room, roomAttributes);
                }
            });

            return new RoomResultModel { Data = rooms, Count = 1, Successful = true, Error = null };
        }
    }
}
