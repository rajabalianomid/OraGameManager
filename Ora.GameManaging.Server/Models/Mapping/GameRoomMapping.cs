using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server.Data;

namespace Ora.GameManaging.Server.Models.Mapping
{
    public static class GameRoomMapping
    {
        public static GameRoomDto ToModel(this GameRoomEntity entity)
        {
            return new GameRoomDto
            {
                AppId = entity.AppId,
                RoomId = entity.RoomId,
                TurnDurationSeconds = entity.TurnDurationSeconds,
                CurrentTurnPlayers = entity.CurrentTurnPlayer ?? string.Empty,
                CreatedAt = entity.CreatedAt.ToString("o")
            };
        }

        public static GameRoomEntity ToEntity(this GameRoomDto model)
        {
            return new GameRoomEntity
            {
                AppId = model.AppId,
                RoomId = model.RoomId,
                TurnDurationSeconds = model.TurnDurationSeconds,
                CurrentTurnPlayer = model.CurrentTurnPlayers,
                CreatedAt = DateTime.Parse(model.CreatedAt),
            };
        }
    }
}
