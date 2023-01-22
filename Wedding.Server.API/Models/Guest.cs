namespace Wedding.Server.API.Models;

public class Guest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Type { get; set; }

    public IEnumerable<Gift>? Gifts { get; set; }
}