using Ineditta.Application.Clausulas.Clientes.Entities;
using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Application.GruposEconomicos.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Clausulas.Clientes
{
    internal sealed class ClausulaClienteMap : IEntityTypeConfiguration<ClausulaCliente>
    {
        public void Configure(EntityTypeBuilder<ClausulaCliente> builder)
        {
            builder.ToTable("clausula_cliente_tb");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.ClausulaId)
                .HasColumnName("clausula_id");

            builder.Property(x => x.Texto)
                .HasColumnName("texto");

            builder.Property(x => x.GrupoEconomicoId)
                .HasColumnName("grupo_economico_id");

            builder.HasOne<ClausulaGeral>()
                .WithMany()
                .HasForeignKey(c => c.ClausulaId)
                .HasConstraintName("fk_clausula_cliente_tb_clausula_geral");

            builder.HasOne<GrupoEconomico>()
                .WithMany()
                .HasForeignKey(c => c.GrupoEconomicoId)
                .HasConstraintName("fk_clausula_cliente_tb_cliente_grupo");
        }
    }
}
