using Microsoft.EntityFrameworkCore;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.Repositories;

public interface IGiftRepository
{
    Task<IEnumerable<Gift>> SelectAsync();
    Task<IEnumerable<Gift>> SelectAllByGuestAsync(int guestId);
    Task<Gift> SelectAsync(int id);
    Task UpdateAsync(Gift gift);
}

public class GiftRepository : IGiftRepository
{
    private readonly ApplicationDbContext _context;
    

    public GiftRepository(ApplicationDbContext context) 
    {
        _context = context;
    }

    public async Task<IEnumerable<Gift>> SelectAllByGuestAsync(int guestId)
    {
        return await _context.Gifts.Include(x => x.Guests).Where(x => x.Guests.Any(y => y.Id == guestId)).ToListAsync();
    }

    public async Task<IEnumerable<Gift>> SelectAsync()
    {
        return await _context.Gifts.Include(x => x.Metadata).Include(x => x.Guests).ToListAsync();
    }

    public async Task<Gift> SelectAsync(int id)
    {
        return await _context.Gifts.Include(x => x.Guests).Include(x => x.Metadata).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(Gift gift)
    {
        _context.Gifts.Update(gift);
        await _context.SaveChangesAsync();
    }
}