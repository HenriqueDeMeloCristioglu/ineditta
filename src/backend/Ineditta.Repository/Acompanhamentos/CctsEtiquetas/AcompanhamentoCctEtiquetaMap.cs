using Ineditta.Application.Acompanhamentos.CctsEtiquetas.Entities;
using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsEtiquetas
{
    internal sealed class AcompanhamentoCctEtiquetaMap : IEntityTypeConfiguration<AcompanhamentoCctEtiqueta>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctEtiqueta> builder)
        {
            builder.ToTable("acompanhamento_cct_etiqueta_tb");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.Property("AcompanhamentoCctId")
                .HasColumnName("acompanhamento_cct_id");

            builder.Property(a => a.AcompanhamentoCctEtiquetaOpcaoId)
                .HasColumnName("acompanhamento_cct_etiqueta_opcao_id");

            builder.HasOne<AcompanhamentoCctEtiquetaOpcao>()
                .WithMany()
                .HasForeignKey(a => a.AcompanhamentoCctEtiquetaOpcaoId)
                .HasConstraintName("fk_acompanhamento_cct_etiqueta_x_acompanhamento_cct_etiqueta_opcao");
        }
    }
}
