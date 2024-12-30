using api_intranet_surfland.DataBase;
using api_intranet_surfland.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace api_intranet_surfland.Controllers; 
[ApiController]
[Route("Users")]
public class UsersController : ControllerBase {

    [Authorize]
    [HttpGet("{userId}")]
    public IActionResult GetUser(int userId) {
        var user = DBUsers.GetUser(userId);

        if (user == null) {
            return NotFound(new IResponse {
                status = false,
                message = "Não foi encontrado nenhum usuário com este Id"
            });
        } else {
            return Ok(new IResponse {
                status = true,
                data = user
            });
        }
    }

    [Authorize]
    [HttpGet("Find")]
    public IActionResult FindUsers(
        [FromQuery] string? login,
        [FromQuery] string? accessProfileId,
        [FromQuery] int? statusId,
        [FromQuery] int? personId,
        [FromQuery] DateTime? updatedAt,
        [FromQuery] int? updatedBy
    ) {
        Console.WriteLine(login);
        List<DTOUser> users = DBUsers.FindUsers(login, accessProfileId, statusId, personId, updatedAt, updatedBy);

        if (users is null || users.Count == 0) {
            return NotFound(new IResponse {
                status = false,
                message = "Não foi encontrado nenhuma pessoa com estas informações",
            });
        } else {
            return Ok(new IResponse {
                status = true,
                data = users
            });
        }
    }
}