using Ineditta.BuildingBlocks.Core.Web.API.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Audits
{
    public class AuditTbMap : IEntityTypeConfiguration<AuditTb>
    {
        public void Configure(EntityTypeBuilder<AuditTb> builder)
        {
            builder.ToTable(nameof(AuditTb).ToSnakeCase());

            builder.HasKey(p => p.Id)
                   .HasName("PRIMARY");

            builder.Property(p => p.Id);

            builder.Property(p => p.OldValues)
                .HasColumnType("json");

            builder.Property(p => p.NewValues)
                .HasColumnType("json");

            builder.Property(p => p.PrimaryKey)
                .HasColumnType("json");

            builder.Property(p => p.AffectedColumns)
                .HasColumnType("json");

            builder.Property(p => p.TableName)
                   .HasMaxLength(300);

            builder.Property(p => p.UserId)
                   .HasMaxLength(50);

            builder.Property(p => p.Type)
                   .HasMaxLength(300);

            builder.Property(p => p.DateTime)
                   .HasColumnType("timestamp")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
