public class TurnInfoModel
{
    public string Phase { get; set; } = "";
    public int Round { get; set; }
    public bool CanSpeak { get; set; }
    public List<string>? Abilities { get; set; }
    public RoleStatusModel? RoleStatus { get; set; }
}

public class RoleStatusModel
{
    public string RoleName { get; set; } = "";
    public int Health { get; set; }
    public int AbilityCount { get; set; }
    public int SelfAbilityCount { get; set; }
    public bool HasNightAbility { get; set; }
    public bool HasDayAbility { get; set; }
    public bool CanSpeak { get; set; }
    public bool DarkSide { get; set; }
    public string Abilities { get; set; } = "";
}