using Library.Api.Contracts.Requests;
using Library.Api.Mapping;
using Library.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [HttpPost]
    public IActionResult Register(RegisterUserRequest request)
    {
        var user = _users.Register(request.Name, request.Roles);

        return CreatedAtAction(nameof(GetByName), new { name = user.Name }, user.ToResponse());
    }

    [HttpGet("{name}")]
    public IActionResult GetByName(string name)
    {
        var user = _users.GetByName(name);

        return Ok(user.ToResponse());
    }
}
