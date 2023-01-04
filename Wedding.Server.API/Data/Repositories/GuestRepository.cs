using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wedding.Server.API.Models;
using Wedding.Server.API.Options;

namespace Wedding.Server.API.Data.Repositories;

public interface IGuestRepository
{
    Task<Guest?> SelectAsync(string code);
    Task<IEnumerable<Guest>> SelectAsync();
}

public class GuestRepository : IGuestRepository
{
    private readonly ApplicationDbContext _context;

    public GuestRepository(ApplicationDbContext context) 
    {
        _context = context;
    }

    public async Task<Guest?> SelectAsync(string code)
    {
        return await _context.Guests.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<IEnumerable<Guest>> SelectAsync()
    {
        return await _context.Guests.ToListAsync();
    }
}