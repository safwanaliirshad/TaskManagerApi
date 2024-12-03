using EntityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManagementAPI.Services
{
    public class TokenService
    {
        public static string GenerateToken(IdentityUser user, UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

            var roles = userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Add roles to claims
            //var roles = _userManager.GetRolesAsync(user).Result;
            //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //var claims = new[]
            //{
            //new Claim(ClaimTypes.Name, user.Username),
            //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //new Claim(ClaimTypes.Role, user.Role.ToString())
        //};

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("58d5g8df5g5d5f47f7g8d5fg54g8d4gf54d84fv5d4f84g5d48f45g4f54f8v45d4d54f84v54d8f44v"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TaskManagetmentapi",
                audience: "TaskManagetmentapi",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
