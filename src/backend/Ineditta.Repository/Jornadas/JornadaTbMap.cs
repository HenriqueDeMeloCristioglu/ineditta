using Ineditta.Application.Jornada.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Jornadas
{
    internal sealed class JornadaTbMap : IEntityTypeConfiguration<Jornada>
    {
        public void Configure(EntityTypeBuilder<Jornada> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("jornada")
                .HasCharSet("utf8mb3")
                    .UseCollation("utf8mb3_general_ci");

            builder.Property(p => p.Id)
                .HasColumnName("id_jornada");

            builder.Property(e => e.JornadaSemanal)
                .HasColumnType("json")
                .HasColumnName("jornada_semanal");

            builder.Property(e => e.Descricao)
                .HasColumnName("descricao");

            builder.Property(e => e.IsDeault)
                .HasColumnName("is_default");
        }
    }
}
