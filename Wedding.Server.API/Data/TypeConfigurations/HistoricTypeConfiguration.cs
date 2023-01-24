using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wedding.Server.API.Models;

namespace Wedding.Server.API.Data.TypeConfigurations;

public class HistoricTypeConfiguration : IEntityTypeConfiguration<Historic>
{
    public void Configure(EntityTypeBuilder<Historic> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatedAt).HasColumnType("DATETIME");
        builder.Property(x => x.AditionalData).HasColumnType("VARCHAR(MAX)");
        builder.Property(x => x.Type).HasColumnType("VARCHAR(150)");
    }
}