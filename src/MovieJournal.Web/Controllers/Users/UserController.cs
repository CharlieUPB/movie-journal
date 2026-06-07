using Microsoft.AspNetCore.Mvc;
using MovieJournal.Application.Users.Commands;
using MovieJournal.Application.Users.Requests;

namespace MovieJournal.Web.Controllers.Users;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly RegisterUserCmd _registerUserCmd;
    private readonly LoginUserCmd _loginUserCmd;

    public UserController(
        RegisterUserCmd registerUserCmd,
        LoginUserCmd loginUserCmd)
    {
        _registerUserCmd = registerUserCmd;
        _loginUserCmd = loginUserCmd;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserHttpRequest body)
    {
        var result = await _registerUserCmd.Execute(
            new RegisterUserRequest(
                body.DisplayName,
                body.Email,
                body.Password));

        return Created("/api/users/register", result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserHttpRequest body)
    {
        var result = await _loginUserCmd.Execute(
            new LoginUserRequest(
                body.Email,
                body.Password));

        return Ok(result);
    }
}
