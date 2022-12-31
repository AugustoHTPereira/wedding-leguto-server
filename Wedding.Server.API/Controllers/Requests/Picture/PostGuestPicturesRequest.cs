namespace Wedding.Server.API.Controllers.Requests;

public class PostGuestPicturesRequest
{
    public ICollection<Picture> Pictures { get; set; }
}

public class Picture
{
    public bool Public { get; set; }
    public IFormFile File { get; set; }
}