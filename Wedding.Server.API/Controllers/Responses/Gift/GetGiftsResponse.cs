using GiftModel = Wedding.Server.API.Models.Gift;

namespace Wedding.Server.API.Controllers.Responses.Gift;

public class GetGiftsResponse
{
    public IEnumerable<GiftViewModel> Gifts { get; set; }
}

public class GiftViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string Store { get; set; }
    public bool Obtained { get; set; }
}