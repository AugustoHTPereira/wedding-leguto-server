using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.Repositories;

public interface IGuestAccessRepository
{
    Task Insert(GuestAccess model);
}

public class GuestAccessRepository : IGuestAccessRepository
{
    private readonly ApplicationDbContext _context;

    public GuestAccessRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Insert(GuestAccess model)
    {
        await _context.GuestAccess.AddAsync(model);
        await _context.SaveChangesAsync();
    }
}