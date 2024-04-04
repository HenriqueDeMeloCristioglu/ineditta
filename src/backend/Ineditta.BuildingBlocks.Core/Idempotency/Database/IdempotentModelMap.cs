using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.BuildingBlocks.Core.Idempotency.Database
{
    public class IdempotentModelMap : IEntityTypeConfiguration<IdempotentModel>
    {
        public void Configure(EntityTypeBuilder<IdempotentModel> builder)
        {
            builder.ToTable("idempotent_tb");

            builder.HasKey(p => p.Id)
                .HasName("pk_idempotent_tb");

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Nome)
                .HasMaxLength(1_000);

            builder.Property(p => p.DataProcessamento)
                .HasColumnName("data_processamento")
                .HasColumnType("DATETIME");
        }
    }
}
