namespace Ora.GameManaging.Mafia.Model;
public class GameRoomModel
{
    public required string AppId { get; set; }
    public required string RoomId { get; set; }
    public int TurnDurationSeconds { get; set; }
    public string? CurrentTurnPlayers { get; set; }
    public DateTime CreatedAt { get; set; }

    // Added properties
    public string? Description { get; set; }
    public string? Owner { get; set; }
    public DateTime? ExpireTime { get; set; }
    public int? MaxPlayer { get; set; }
    public bool? IsChallenge { get; set; }
    public int? SpeakTime { get; set; }
    public int? ChallengeTime { get; set; }
    public int? DefensedTime { get; set; }
    public int? ActionTime { get; set; }
    public int? FirstVoteTime { get; set; }
    public int? FinalVoteTime { get; set; }
    public bool? CanGodFatherShowByDetective { get; set; }
    public bool? HasMeeting { get; set; }
    public DateTime? ExpireTimeMeeting { get; set; }
}