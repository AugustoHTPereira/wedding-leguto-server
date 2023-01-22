namespace Wedding.Server.API.Models;

public class GiftMetadata
{
    public int Id { get; set; }
    public Gift Gift { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}