namespace Ora.GameManaging.Server.Models
{
    public class LatestInformationModel
    {
        public string? Phase { get; set; }
        public int Round { get; set; }
        public bool CanSpeak { get; set; }
        public int RemindTime { get; set; }
        public List<string>? Abilities { get; set; }
        public RoleStatusModel? RoleStatus { get; set; }
    }
}
