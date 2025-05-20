using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ora.GameManaging.Mafia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(IConfiguration configuration,
        SignInManager<ApplicationUser> signInManager) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var jwtSecurityKey = configuration["JwtSecurityKey"];
            if (string.IsNullOrEmpty(jwtSecurityKey))
            {
                return StatusCode(500, "JWT Security Key is not configured.");
            }

            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

            if (!result.Succeeded) return Ok(new LoginResultModel { Successful = false, Error = "Username and password are invalid." });

            var user = await signInManager.UserManager.FindByEmailAsync(login.Email);

            if (user == null) // Ensure user is not null
            {
                return StatusCode(500, "User not found.");
            }

            var roles = await signInManager.UserManager.GetRolesAsync(user);

            var claims = new List<Claim> { new(ClaimTypes.Name, login.Email), new(ClaimTypes.NameIdentifier, login.Email) };

            await signInManager.UserManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, login.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(configuration["JwtExpiryInDays"]));
            var expiryUtc = DateTime.UtcNow.AddDays(Convert.ToInt32(configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                configuration["JwtIssuer"],
                configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return Ok(new LoginResultModel { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token), ImageName = user.Id, ExpireTime = expiryUtc, UserName = user.UserName });
        }
    }
}
