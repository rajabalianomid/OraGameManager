using Ora.GameManaging.Mafia.Protos;

namespace Ora.GameManaging.Mafia.Model.Mapping;
public static class GameRoomMapping
{
    public static GameRoomModel ToModel(this GameRoomDto room)
    {
        return new GameRoomModel
        {
            AppId = room.AppId,
            RoomId = room.RoomId,
            TurnDurationSeconds = room.TurnDurationSeconds,
            CurrentTurnPlayers = room.CurrentTurnPlayers,
            CreatedAt = DateTime.Parse(room.CreatedAt),
        };
    }

    public static GameRoomDto ToProto(this GameRoomModel model)
    {
        return new GameRoomDto
        {
            AppId = model.AppId ?? "",
            RoomId = model.RoomId ?? "",
            TurnDurationSeconds = model.TurnDurationSeconds,
            CurrentTurnPlayers = model.CurrentTurnPlayers ?? "",
            CreatedAt = model.CreatedAt.ToUniversalTime().ToString("o"),
        };
    }

    //public static GameRoomModel ToModel(this GameRoomModel entity)
    //{
    //    return new GameRoomModel
    //    {
    //        Id = entity.Id,
    //        AppId = entity.AppId,
    //        RoomId = entity.RoomId,
    //        TurnDurationSeconds = entity.TurnDurationSeconds,
    //        CurrentTurnPlayers = entity.CurrentTurnPlayers,
    //        CreatedAt = entity.CreatedAt,
    //        LastSnapshotJson = entity.LastSnapshotJson
    //    };
    //}

    //public static GameRoomEntity ToEntity(this GameRoomModel model)
    //{
    //    return new GameRoomEntity
    //    {
    //        Id = model.Id,
    //        AppId = model.AppId,
    //        RoomId = model.RoomId,
    //        TurnDurationSeconds = model.TurnDurationSeconds,
    //        CurrentTurnPlayers = model.CurrentTurnPlayers,
    //        CreatedAt = model.CreatedAt,
    //        LastSnapshotJson = model.LastSnapshotJson
    //    };
    //}
}