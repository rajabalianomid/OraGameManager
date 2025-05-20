using Microsoft.AspNetCore.Identity;

namespace Ora.GameManaging.Mafia.Data
{
    public class ApplicationUser : IdentityUser
    {
        public required string ApplicationInstanceId { get; set; } // This should match the instance id from Ora.GameManaging.Server
    }
}