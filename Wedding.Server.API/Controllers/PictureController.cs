using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wedding.Server.API.Controllers.Base;
using Wedding.Server.API.Controllers.DefaultResponses;
using Wedding.Server.API.Controllers.Requests;
using Wedding.Server.API.Controllers.Responses.GuestPicture;
using Wedding.Server.API.Data.Repositories;
using Wedding.Server.API.Models;
using Wedding.Server.API.Services;

namespace Wedding.Server.API.Controllers;

public class PictureController : APIControllerBase
{
    private readonly IStorageService _storageService;
    private readonly IGuestRepository _guestRepository;
    private readonly IPictureRepository _pictureRepository;

    public PictureController(IStorageService storageService, IPictureRepository pictureRepository)
    {
        _storageService = storageService;
        _pictureRepository = pictureRepository;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> OnPostAsync([FromForm] PostGuestPicturesRequest request)
    {
        if (request.Pictures.Count() == 0) {
            return BadRequestResponse.CreateResponse("Files are required");
        }

        foreach (var picture in request.Pictures)
        {
            if (!picture.File.ContentType.Contains("image"))
                return BadRequestResponse.CreateResponse($"File type {picture.File.ContentType} is not supported");
        }

        foreach (var picture in request.Pictures)
        {   
            string fileKey = $"{Guid.NewGuid()}{Path.GetExtension(picture.File.FileName)}";
            var guestPicture = new GuestPicture
            {
                ContentType = picture.File.ContentType,
                Guest = new Guest 
                {
                    Id = Id
                },
                Name = fileKey,
                Public = picture.Public,
                Size = picture.File.Length,
                BucketName = "wedding-guest-photos",    
            };

            guestPicture.StorageObjectId = await _storageService.StoreAsync(fileKey, picture.File, guestPicture.BucketName);
            if (!string.IsNullOrEmpty(guestPicture.StorageObjectId))
            {
                await _pictureRepository.InsertAsync(guestPicture);
            }
        }
        
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> OnGet([FromQuery] int guestId)
    {
        if (guestId == 0)
            return await OnGetRandomPublicPictures();
        else
            return await OnGetGuestPictures(guestId, !IsLoggedIn);
    }

    protected async Task<IActionResult> OnGetRandomPublicPictures()
    {
        var pictures = await _pictureRepository.SelectPublicRandomAsync(10);

        IList<PictureResponse> resp = new List<PictureResponse>();
        foreach (var picture in pictures)
        {
            string originalUrl = _storageService.GetUrlAsync(picture.Name, picture.BucketName);
            var pr = new PictureResponse
            {
                Owner = picture.Guest.Name,
                OriginalUrl = originalUrl,
                Id = picture.Id,
            };
            
            resp.Add(pr);
        }

        return Ok(resp);
    }

    protected async Task<IActionResult> OnGetGuestPictures(int guestId, bool onlyPublic)
    {
        var pictures = await _pictureRepository.SelectAllByGuestAsync(guestId, onlyPublic);

        IList<PictureResponse> resp = new List<PictureResponse>();
        foreach (var picture in pictures)
        {
            string originalUrl = _storageService.GetUrlAsync(picture.Name, picture.BucketName);
            var pr = new PictureResponse
            {
                Owner = picture.Guest.Name,
                OriginalUrl = originalUrl,
                Id = picture.Id,
            };
            
            resp.Add(pr);
        }

        return Ok(resp);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> OnDeleteAsync([FromRoute] int id)
    {
        var picture = await _pictureRepository.SelectAsync(id);
        if (picture == null)
            return NotFound();
            
        if (picture.Guest.Id != Id)
        {
            return Forbid();
        }

        await _storageService.DeleteAsync(picture.Name, picture.BucketName);
        await _pictureRepository.DeleteAsync(picture);
        return Ok();
    }

    [Authorize]
    [HttpPut("{id}/TogglePrivacy")]
    public async Task<IActionResult> OnUpdateAsync([FromRoute] int id)
    {
        var picture = await _pictureRepository.SelectAsync(id);
        if (picture.Guest.Id != Id)
        {
            return Forbid();
        }

        picture.Public = !picture.Public;
        await _pictureRepository.UpdateAsync(picture);
        return Ok();
    }
}