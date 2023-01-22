using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wedding.Server.API.Models;
using Wedding.Server.API.Options;

namespace Wedding.Server.API.Data;

public class ApplicationDbContext : DbContext
{
    private string _connectionString { get; }

    public ApplicationDbContext(IOptions<SqlServerOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public DbSet<Guest> Guests { get; set; }
    public DbSet<GuestPicture> GuestPictures { get; set; }
    public DbSet<Gift> Gifts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}