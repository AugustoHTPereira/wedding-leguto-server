using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.Repositories;

public interface IHistoricRepository
{
    Task Insert(Historic model);
}

public class HistoricRepository : IHistoricRepository
{
    private readonly ApplicationDbContext _context;

    public HistoricRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Insert(Historic model)
    {
        await _context.Historical.AddAsync(model);
        await _context.SaveChangesAsync();
    }
}