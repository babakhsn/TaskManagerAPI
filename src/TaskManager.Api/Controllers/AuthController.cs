using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Contracts;
using TaskManager.Application.Abstractions;
using TaskManager.Infrastructure.Identity;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwt;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwt)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => $"{e.Code}: {e.Description}");
            return BadRequest(new { Errors = errors });
        }
        var token = _jwt.GenerateToken(user.Id, user.Email!);
        return Ok(new AuthResponse(token));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded) return Unauthorized();

        var token = _jwt.GenerateToken(user.Id, user.Email!);
        return Ok(new AuthResponse(token));
    }
}
