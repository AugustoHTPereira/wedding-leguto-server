using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.TypeConfigurations;

public class GiftTypeConfiguration : IEntityTypeConfiguration<Gift>
{
    public void Configure(EntityTypeBuilder<Gift> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasColumnType("varchar(255)");
        builder.Property(x => x.Link).HasColumnType("varchar(255)");
        builder.Property(x => x.Store).HasColumnType("varchar(255)");
        builder.Property(x => x.Type).HasColumnType("varchar(255)");
    }
}