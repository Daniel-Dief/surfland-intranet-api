using api_intranet_surfland.DataBase;
using api_intranet_surfland.Models;
using api_intranet_surfland.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_intranet_surfland.Controllers;
[ApiController]
[Route("Authenticate")]
public class AuthController : ControllerBase {
    [HttpPost]
    public IActionResult Auth([FromBody] IAuthCredentials credentials) {

        List<DTOUser> user = DBUsers.FindUsers(login : credentials.login);

        if (credentials.login == "admin" && credentials.password == "admin") {
            var token = TokenService.GenerateToken(new DTOUser());
            return Ok(new IResponse {
                status = true,
                data = token
            });
        }

        return BadRequest(new IResponse {
            status = false,
            message = "Usuário ou senha inválidos"
        });
    }
}