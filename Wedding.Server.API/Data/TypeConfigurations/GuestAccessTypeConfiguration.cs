using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.TypeConfigurations;

public class GuestAccessTypeConfiguration : IEntityTypeConfiguration<GuestAccess>
{
    public void Configure(EntityTypeBuilder<GuestAccess> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Guest);
        builder.Property(x => x.CreatedAt).HasColumnType("DATETIME");
    }
}