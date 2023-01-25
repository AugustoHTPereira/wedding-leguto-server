using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.TypeConfigurations
{
    public class GiftMediaTypeConfiguration : IEntityTypeConfiguration<GiftMedia>
    {
        public void Configure(EntityTypeBuilder<GiftMedia> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Gift);
            builder.Property(x => x.Url).HasColumnType("varchar(255)");
        }
    }
}
