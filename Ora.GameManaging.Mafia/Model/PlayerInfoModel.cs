namespace Ora.GameManaging.Mafia.Model
{
    public class PlayerInfoModel
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        private string _role = string.Empty;
        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                if (RoleStatus != null)
                {
                    RoleStatus.RoleName = value;
                }
            }
        }
        public RoleStatusModel RoleStatus { get; set; } = default!;
        public int Status { get; set; }
        public bool IsAlive => RoleStatus.Health > 0;
    }
}