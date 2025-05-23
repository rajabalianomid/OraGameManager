using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server.Data;

namespace Ora.GameManaging.Server.Model.Mapping
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
                CurrentTurnPlayers = entity.CurrentTurnPlayers ?? string.Empty,
                CreatedAt = entity.CreatedAt.ToString("o"),
            };
        }

        public static GameRoomEntity ToEntity(this GameRoomDto model)
        {
            return new GameRoomEntity
            {
                AppId = model.AppId,
                RoomId = model.RoomId,
                TurnDurationSeconds = model.TurnDurationSeconds,
                CurrentTurnPlayers = model.CurrentTurnPlayers,
                CreatedAt = DateTime.Parse(model.CreatedAt),
            };
        }
    }
}
