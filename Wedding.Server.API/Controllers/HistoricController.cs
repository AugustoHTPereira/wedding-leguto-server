using Microsoft.AspNetCore.Mvc;
using Wedding.Server.API.Controllers.Base;
using Wedding.Server.API.Data.Repositories;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Controllers;

public class HistoricController : APIControllerBase
{
    private readonly IHistoricRepository _historicRepository;

    public HistoricController(IHistoricRepository historicRepository)
    {
        _historicRepository = historicRepository;
    }

    [HttpGet("{type}")]
    public async Task<IActionResult> OnPostAsync(
        [FromRoute] string type,
        [FromQuery] string? message,
        [FromQuery] string? aditionalData
    )
    {
        var model = new Historic
        {
            AditionalData = aditionalData ?? "",
            CreatedAt = DateTime.Now,
            Message = message ?? "",
            Type = type,
        };

        await _historicRepository.Insert(model);

        return NoContent();
    }
}