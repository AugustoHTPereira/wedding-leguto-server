using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wedding.Server.API.Controllers.Base;
using Wedding.Server.API.Controllers.Responses.Gift;
using Wedding.Server.API.Data.Repositories;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Controllers;

public class GiftController : APIControllerBase
{
    private readonly IGiftRepository _giftRepository;
    private readonly IGuestRepository _guestRepository;

    public GiftController(IGiftRepository giftRepository, IGuestRepository guestRepository)
    {
        _giftRepository = giftRepository;
        _guestRepository = guestRepository;
    }

    [HttpGet]
    public async Task<IActionResult> OnGetGifts() 
    {
        var gifts = await _giftRepository.SelectAsync();
        return Ok(gifts.Select(x => new GiftViewModel {
                Id = x.Id,
                Link = x.Link,
                Obtained = x.Guests?.Any() ?? false,
                Title = x.Title,
                Store = x.Store
            }));
    }

    [Authorize]
    [HttpPost("{giftId:int}/Take")]
    public async Task<IActionResult> OnTakeGift([FromRoute] int giftId)
    {
        var gift = await _giftRepository.SelectAsync(giftId);
        if (gift == null)
            return NotFound();

        var guest = await _guestRepository.SelectAsync(Id);
        if (guest == null)
            return NotFound();

        if (gift.Guests != null && gift.Guests.Any())
            return BadRequest(new {
                message = "Gift was taken",
            });

        gift.Guests = gift.Guests ?? new List<Guest>();
        gift.Guests.Add(guest);

        await _giftRepository.UpdateAsync(gift);
        return Ok();
    }
}