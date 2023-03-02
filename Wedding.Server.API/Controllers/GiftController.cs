using AngleSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
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
        return Ok(gifts.Select(x => new GiftViewModel
        {
            Id = x.Id,
            Link = x.Link,
            Obtained = x.Guests?.Any() ?? false,
            ObtainedByMe = x.Guests?.Any(y => y.Id == Id) ?? false,
            Title = x.Title,
            Store = x.Store,
            Type = x.Type,
            Metadata = x.Metadata.Select(y => new KeyValuePair<string, string>(y.Key, y.Value)),
            Pictures = x.Media.Select(x => x.Url),
        }));
    }

    [HttpGet("{giftId:int}")]
    public async Task<IActionResult> OnGetGiftDetails([FromRoute] int giftId)
    {
        var gift = await _giftRepository.SelectAsync(giftId);
        if (gift == null)
            return NotFound();

        return Ok(new
        {
            obtained = gift.Guests != null && gift.Guests.Any(),
            gift.Id,
            gift.Link,
            gift.Title,
            gift.Store,
            GuestsId = gift.Guests?.Select(x => x.Id),
            Type = gift.Type,
            Metadata = gift.Metadata.Select(y => new KeyValuePair<string, string>(y.Key, y.Value)),
            Pictures = gift.Media.Select(x => x.Url),
        });
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
            return BadRequest(new
            {
                message = "Gift was taken",
            });

        gift.Guests = gift.Guests ?? new List<Guest>();
        gift.Guests.Add(guest);

        await _giftRepository.UpdateAsync(gift);
        return Ok();
    }

    [Authorize]
    [HttpDelete("{giftId:int}/Take")]
    public async Task<IActionResult> OnUntakeGift([FromRoute] int giftId)
    {
        var gift = await _giftRepository.SelectAsync(giftId);
        if (gift == null)
            return NotFound();

        var guest = await _guestRepository.SelectAsync(Id);
        if (guest == null)
            return NotFound();

        gift.Guests = gift.Guests ?? new List<Guest>();
        gift.Guests.Remove(guest);

        await _giftRepository.UpdateAsync(gift);
        return Ok();
    }

    [Authorize]
    [HttpGet("Take")]
    public async Task<IActionResult> OnGetTakedGifts()
    {
        var guestGifts = await _giftRepository.SelectAllByGuestAsync(Id);
        if (guestGifts == null)
            return NotFound();

        return Ok(guestGifts.Select(x => new GiftViewModel
        {
            Id = x.Id,
            Link = x.Link,
            Obtained = x.Guests?.Any() ?? false,
            Title = x.Title,
            Store = x.Store
        }));
    }

    protected async Task<string[]> CrawlPictures(Gift gift)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(gift.Link);
        var responseContent = await response.Content.ReadAsStringAsync();
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(responseContent));

        IList<string> pictureUrls = new List<string>();

        if (gift.Store == "AMAZON")
        {
            var image = document.Body.QuerySelectorAll("*").FirstOrDefault(x => x.Id == "landingImage")?.Attributes["src"];
            if (image != null)
                pictureUrls.Add(image.Value);
        }

        return pictureUrls.ToArray();
    }
}