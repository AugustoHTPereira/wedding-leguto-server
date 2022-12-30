using Wedding.Server.API.Data.Repositories;
using Wedding.Server.API.Options;
using Wedding.Server.API.Services;

namespace Wedding.Server.API;

public static class Bootstrapper
{
    public static void AddApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGuestRepository, GuestRepository>();
    }

    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }
}