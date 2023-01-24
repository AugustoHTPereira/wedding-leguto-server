namespace Wedding.Server.API.Models;

public class Historic
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AditionalData { get; set; }
}

public class HistoricType
{
    public const string GiftVisit = "gift_visit";
    public const string WebsiteAccess = "public_access";
}