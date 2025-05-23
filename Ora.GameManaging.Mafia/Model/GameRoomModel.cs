namespace Ora.GameManaging.Mafia.Model;
public class GameRoomModel
{
    public required string AppId { get; set; }
    public required string RoomId { get; set; }
    public int TurnDurationSeconds { get; set; }
    public string? CurrentTurnPlayers { get; set; }
    public DateTime CreatedAt { get; set; }
}