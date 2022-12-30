using Microsoft.AspNetCore.Mvc;
using Wedding.Server.API.Controllers.Base;

namespace Wedding.Server.API.Controllers;

public class PingController : APIControllerBase
{
    [HttpGet]
    public IActionResult OnGet()
    {
        return Ok("Ok");
    }
}