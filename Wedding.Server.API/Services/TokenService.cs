using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wedding.Server.API.Models;
using Wedding.Server.API.Options;

namespace Wedding.Server.API.Services;

public interface ITokenService
{
    string CreateAccessToken(Guest guest);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string CreateAccessToken(Guest guest)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, guest.Name.ToString()),
                new Claim(ClaimTypes.Role, guest.Type),
                new Claim(ClaimTypes.NameIdentifier, guest.Id.ToString()),
            }),
            Expires = DateTime.UtcNow.AddHours(_options.Expires),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}