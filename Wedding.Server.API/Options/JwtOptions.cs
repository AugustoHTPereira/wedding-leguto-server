namespace Wedding.Server.API.Options;

public class JwtOptions
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public int Expires { get; set; }
}