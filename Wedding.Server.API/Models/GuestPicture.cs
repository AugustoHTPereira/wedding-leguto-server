namespace Wedding.Server.API.Models;

public class GuestPicture
{
    public int Id { get; set; }
    public Guest Guest { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public bool Public { get; set; }
    public string StorageObjectId { get; set; }
    public string BucketName { get; set; }
}