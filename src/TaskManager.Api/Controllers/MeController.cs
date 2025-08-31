using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/me")]
public class MeController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get() => Ok(new { Message = "You are authorized!" });
}
