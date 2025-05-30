namespace Ora.GameManaging.Mafia.Model
{
    public class RoleActionModel
    {
        public required string RoleName { get; set; }
        public required string Expression { get; set; } // Example: "target.Health += 1"
        public required string Condition { get; set; }  // Example: "target.IsAlive"
        public required string PropertyName { get; set; }
    }
}
