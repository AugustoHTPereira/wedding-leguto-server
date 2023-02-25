using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.TypeConfigurations;

public class GuestTypeConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.ToTable("Guests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnType("varchar(255)");
        builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");
        builder.Property(x => x.Extensive).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}