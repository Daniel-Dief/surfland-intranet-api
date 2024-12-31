using api_intranet_surfland.Models;
using System.Reflection.PortableExecutable;

namespace api_intranet_surfland.Services;
public class AuthMiddleware {
    private readonly RequestDelegate _next;
    public AuthMiddleware(RequestDelegate next) {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context) {
        if (
            context.Request.Path.Value.ToLower().Contains("/swagger") ||
            context.Request.Path.Value.Contains("/Authenticate")
        ) {
            await _next(context);
            return;
        }

        var token = GetTokenFromHeader(context.Request.Headers);

        if (token == null) {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new IResponse {
                    status = false,
                    message = "Token not provided"
                }
            );
            return;
        }

        var claimsPrincipal = TokenService.ValidateToken(token);

        if (claimsPrincipal == null) {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new IResponse {
                    status = false,
                    message = "Invalid token"
                }    
            );
            return;
        }

        context.User = claimsPrincipal;

        await _next(context);
    }

    private string GetTokenFromHeader(IHeaderDictionary headers) {
        if (headers.TryGetValue("Authorization", out var authorizationHeader)) {
            var bearerToken = authorizationHeader.FirstOrDefault();
            if (bearerToken != null && bearerToken.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase)) {
                return bearerToken.Substring(7);
            }
        }
        return null;
    }
}