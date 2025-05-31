namespace Ora.GameManaging.Mafia.Model
{
    public class RoleActionModel
    {
        public required string RoleName { get; set; }
        public required string Expression { get; set; } 
    }
    public class ScriptGlobals
    {
        public required PlayerInfoModel target { get; set; }
    }
}
