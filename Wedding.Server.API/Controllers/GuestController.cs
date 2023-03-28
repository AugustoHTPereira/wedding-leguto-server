using System.Drawing;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
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
    private readonly IGuestAccessRepository _guestAccessRepository;

    public GuestController(IGuestRepository guestRepository, ITokenService tokenService, IGuestAccessRepository guestAccessRepository)
    {
        _guestRepository = guestRepository;
        _tokenService = tokenService;
        _guestAccessRepository = guestAccessRepository;
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
        await _guestAccessRepository.Insert(new GuestAccess { Guest = guest, CreatedAt = DateTime.Now });
        return Ok(new GuestLoginResponse { AccessToken = token });
    }

    // [Authorize]
    [HttpGet("Invitement")]
    public async Task<IActionResult> GenerateInvites([FromServices] IInvitementService invitementService)
    {
        var guests = await _guestRepository.SelectAsync();
        string physical = invitementService.GenerateInvites(guests.Where(x => x.Type == "guest"), InvitementType.A4);
        string digital = invitementService.GenerateInvites(guests.Where(x => x.Type == "digital"), InvitementType.A5);
        return Ok(new {
            physical_invitement_path = physical,
            digital_invitement_path = digital,
        });
    }
}