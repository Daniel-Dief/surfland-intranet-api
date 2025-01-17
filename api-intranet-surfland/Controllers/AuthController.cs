﻿using api_intranet_surfland.DataBase;
using api_intranet_surfland.Models;
using api_intranet_surfland.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_intranet_surfland.Controllers;
[ApiController]
[Route("Authenticate")]
public class AuthController : ControllerBase {
    [HttpPost]
    public IActionResult Auth([FromBody] IAuthCredentials credentials) {
        List<DTOUser> userList = DBUsers.FindUsers(login : credentials.login);

        if (userList.Count == 0) {
            return BadRequest(new IResponse {
                status = false,
                message = "Usuário ou senha inválidos"
            });
        }

        DTOUser user = userList[0];

        if (user.Password == credentials.password || user.TemporaryPassword == credentials.password) {
        var token = TokenService.GenerateToken(user);

        Response.Headers.Add("Authorization", $"Bearer {token}");

        return Ok(new IResponse {
            status = true,
            data = user
        });
        }

        return BadRequest(new IResponse {
            status = false,
            message = "Usuário ou senha inválidos"
        });
    }
}