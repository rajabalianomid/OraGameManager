namespace Ora.GameManaging.Mafia.Data;

public class RoleStatusesAbilityEntity
{
    public int Id { get; set; }
    public int RoleStatusId { get; set; }
    public int AbilityId { get; set; }
    public bool AddedLater { get; set; }

    public RoleStatusEntity RoleStatus { get; set; } = default!;
    public AbilityEntity Ability { get; set; } = default!;
}