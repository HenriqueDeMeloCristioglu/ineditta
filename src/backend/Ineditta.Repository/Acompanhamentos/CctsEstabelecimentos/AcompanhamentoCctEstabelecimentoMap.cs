using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Entities;
using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.GruposEconomicos.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsEstabelecimentos
{
    internal sealed class AcompanhamentoCctEstabelecimentoMap : IEntityTypeConfiguration<AcompanhamentoCctEstabelecimento>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctEstabelecimento> builder)
        {
            builder.ToTable("acompanhamento_cct_estabelecimento_tb");

            builder.HasKey(a => a.Id)
                .HasName("PRIMARY");

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.HasIndex(a => a.AcompanhamentoCctId, "ix001_acompanhamento_cct_estabelecimento_tb");

            builder.HasIndex(a => new { a.AcompanhamentoCctId, a.EstabelecimentoId, a.GrupoEconomicoId, a.EmpresaId }, "ix002_acompanhamento_cct_estabelecimento_tb")
                .IsUnique();

            builder.Property(a => a.AcompanhamentoCctId)
                .HasColumnName("acompanhamento_cct_id");

            builder.Property(a => a.GrupoEconomicoId)
                .HasColumnName("grupo_economico_id");

            builder.Property(a => a.EmpresaId)
                .HasColumnName("empresa_id");

            builder.Property(a => a.EstabelecimentoId)
                .HasColumnName("estabelecimento_id");

            builder.HasOne<AcompanhamentoCct>()
                .WithMany()
                .HasForeignKey(a => a.AcompanhamentoCctId)
                .HasConstraintName("fk_acompanhamento_cct_estabelecimento_x_acompanhamento_cct");

            builder.HasOne<GrupoEconomico>()
                .WithMany()
                .HasForeignKey(a => a.GrupoEconomicoId)
                .HasConstraintName("fk_acompanhamento_cct_estabelecimento_x_cliente_grupo");

            builder.HasOne<ClienteMatriz>()
                .WithMany()
                .HasForeignKey(a => a.EmpresaId)
                .HasConstraintName("fk_acompanhamento_cct_estabelecimento_x_cliente_matriz");

            builder.HasOne<ClienteUnidade>()
                .WithMany()
                .HasForeignKey(a => a.EstabelecimentoId)
                .HasConstraintName("fk_acompanhamento_cct_estabelecimento_x_cliente_unidades");
        }
    }
}
