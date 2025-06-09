namespace Ora.GameManaging.Mafia.Model
{
    public class PlayerInfoModel : BasePlayerInfo
    {
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
        public bool IsAlive => RoleStatus?.Health > 0;
    }
}