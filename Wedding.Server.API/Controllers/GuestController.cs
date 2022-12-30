using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wedding.Server.API.Controllers.Base;
using Wedding.Server.API.Controllers.DefaultResponses;
using Wedding.Server.API.Controllers.Requests.Guest;
using Wedding.Server.API.Controllers.Responses.Guest;
using Wedding.Server.API.Data.Repositories;
using Wedding.Server.API.Models;
using Wedding.Server.API.Services;

namespace Wedding.Server.API.Controllers;

public class GuestController : APIControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IGuestRepository _guestRepository;

    public GuestController(IGuestRepository guestRepository, ITokenService tokenService)
    {
        _guestRepository = guestRepository;
        _tokenService = tokenService;
    }
    
    [Authorize]
    [HttpGet("{code}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Guest), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> OnGetAsync([FromRoute] string code)
    {
        var guest = await _guestRepository.SelectAsync(code);
        if (guest == null)
            return NotFoundResponse.CreateResponse();

        return Ok(guest);
    }

    [HttpPost("login")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GuestLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OnLoginAsync([FromBody] GuestLoginRequest request)
    {
        var guest = await _guestRepository.SelectAsync(request.Code);
        if (guest == null)
            return NotFoundResponse.CreateResponse();

        var token = _tokenService.CreateAccessToken(guest);
        return Ok(new GuestLoginResponse { AccessToken = token });
    }
}