using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.TypeConfigurations;

public class GuestPictureTypeConfiguration : IEntityTypeConfiguration<GuestPicture>
{
    public void Configure(EntityTypeBuilder<GuestPicture> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnType("varchar(255)");
        builder.HasOne(x => x.Guest);
    }
}