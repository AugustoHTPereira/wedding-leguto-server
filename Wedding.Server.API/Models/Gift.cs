namespace Wedding.Server.API.Models;

public class Gift
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Store { get; set; }
    public string? Type { get; set; }

    public IList<GiftMetadata>? Metadata { get; set; }
    public IList<Guest>? Guests { get; set; }
}
