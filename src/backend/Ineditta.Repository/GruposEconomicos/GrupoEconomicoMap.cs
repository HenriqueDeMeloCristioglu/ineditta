using Microsoft.EntityFrameworkCore;
using Ineditta.Application.GruposEconomicos.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.GruposEconomicos
{
    public class GrupoEconomicoMap : IEntityTypeConfiguration<GrupoEconomico>
    {
        public void Configure(EntityTypeBuilder<GrupoEconomico> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("cliente_grupo")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            builder.Property(e => e.Id).HasColumnName("id_grupo_economico");
            builder.Property(e => e.Logotipo).HasColumnName("logo_grupo");
            builder.Property(e => e.Nome)
                .HasColumnType("text")
                .HasColumnName("nome_grupoeconomico");
        }
    }
}
