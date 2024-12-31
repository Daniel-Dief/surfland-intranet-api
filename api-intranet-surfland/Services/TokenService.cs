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
    public static ClaimsPrincipal ValidateToken(string token) {
        try {
            DotNetEnv.Env.Load();
            var key = Encoding.ASCII.GetBytes(
                Environment.GetEnvironmentVariable("TOKEN_KEY")
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                return principal;
            }

            throw new SecurityTokenException("Token inválido");
        } catch (Exception ex) {
            Console.WriteLine($"Erro ao validar token: {ex.Message}");
            return null; // Retorna null se o token for inválido
        }
    }
}