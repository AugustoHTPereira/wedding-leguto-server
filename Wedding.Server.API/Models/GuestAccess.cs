namespace Wedding.Server.API.Models;

public class GuestAccess
{
    public int Id { get; set; }
    public Guest Guest { get; set; }
    public DateTime CreatedAt { get; set; }
}