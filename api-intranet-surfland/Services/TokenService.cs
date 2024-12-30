using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using api_intranet_surfland.Models;
using System.IdentityModel.Tokens.Jwt;

namespace api_intranet_surfland.Services; 
public class TokenService {
    public static object GenerateToken(DTOUser user) {
        DotNetEnv.Env.Load();
        var key = Encoding.ASCII.GetBytes(
            Environment.GetEnvironmentVariable("TOKEN_KEY")
        );
        var tokenConfig = new SecurityTokenDescriptor {
            Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] {
                new Claim("user", user.UserId.ToString() ?? "0"),
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenConfig);
        var tokenString = tokenHandler.WriteToken(token);

        return new {
            token = tokenString
        };
    }
}