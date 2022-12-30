using System.Text.Json.Serialization;

namespace Wedding.Server.API.Controllers.Responses.Guest;

public class GuestLoginResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}