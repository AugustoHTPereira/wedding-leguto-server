using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.TypeConfigurations;

public class GiftMetadataTypeConfiguration : IEntityTypeConfiguration<GiftMetadata>
{
    public void Configure(EntityTypeBuilder<GiftMetadata> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).HasColumnType("varchar(100)");
        builder.Property(x => x.Value).HasColumnType("varchar(100)");
        builder.HasOne(x => x.Gift);
    }
}