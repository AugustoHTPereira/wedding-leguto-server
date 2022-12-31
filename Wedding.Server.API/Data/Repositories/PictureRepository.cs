using Microsoft.EntityFrameworkCore;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.Repositories;

public interface IPictureRepository
{
    Task InsertAsync(GuestPicture model);
    Task DeleteAsync(GuestPicture model);
    Task UpdateAsync(GuestPicture model);
    Task<GuestPicture> SelectAsync(int id);
    Task<IEnumerable<GuestPicture>> SelectAllByGuestAsync(int guestId, bool onlyPublic);
    Task<IEnumerable<GuestPicture>> SelectPublicRandomAsync(int count);
}

public class PictureRepository : IPictureRepository
{
    private readonly ApplicationDbContext _context;

    public PictureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task DeleteAsync(GuestPicture model)
    {
        _context.GuestPictures.Remove(model);
        await _context.SaveChangesAsync();
    }

    public async Task InsertAsync(GuestPicture model)
    {
        model.Guest = await _context.Guests.FindAsync(model.Guest.Id);
        await _context.GuestPictures.AddAsync(model);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GuestPicture>> SelectAllByGuestAsync(int guestId, bool onlyPublic)
    {
        return await _context.GuestPictures.Where(x => x.Guest.Id.Equals(guestId) && (onlyPublic && x.Public == true)).ToListAsync();
    }

    public async Task<GuestPicture> SelectAsync(int id)
    {
        return await _context.GuestPictures.Include(x => x.Guest).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<GuestPicture>> SelectPublicRandomAsync(int count)
    {
        return await _context.GuestPictures
            .Where(x => x.Public)
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .Include(x => x.Guest)
            .ToListAsync();
    }

    public async Task UpdateAsync(GuestPicture model)
    {
        _context.GuestPictures.Update(model);
        await _context.SaveChangesAsync();
    }
}