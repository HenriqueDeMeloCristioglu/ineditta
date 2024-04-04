using Ineditta.Application.EstruturasClausulas.GruposEconomicos.Entities;
using Ineditta.Application.GruposEconomicos.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EstruturasClausulas.GruposEconomicos
{
    internal sealed class EstruturaClausulaGralGrupoEconomicoMap : IEntityTypeConfiguration<EstruturaClausulaGrupoEconomico>
    {
        public void Configure(EntityTypeBuilder<EstruturaClausulaGrupoEconomico> builder)
        {
            builder.ToTable("estrutura_clausula_grupo_economico_tb");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id");

            builder.Property(c => c.GrupoEconomicoId)
                .HasColumnName("grupo_economico_id");

            builder.Property("EstruturaClausulaId")
                .HasColumnName("estrutura_clausula_id");

            builder.HasOne<GrupoEconomico>()
                .WithMany()
                .HasForeignKey(c => c.GrupoEconomicoId)
                .HasConstraintName("fk_estrutura_clausula_grupo_economico_x_cliente_grupo");
        }
    }
}
