using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Wedding.Server.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public class APIControllerBase : ControllerBase
{
    protected bool IsLoggedIn => User.Identity?.IsAuthenticated ?? false;
    protected string Name => User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value ?? "";
    protected int Id => int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
}