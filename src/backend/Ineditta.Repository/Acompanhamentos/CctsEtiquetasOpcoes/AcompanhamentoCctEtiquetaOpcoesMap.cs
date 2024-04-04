using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsEtiquetas
{
    public class AcompanhamentoCctEtiquetaOpcoesMap : IEntityTypeConfiguration<AcompanhamentoCctEtiquetaOpcao>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctEtiquetaOpcao> builder)
        {
            builder.ToTable("acompanhamento_cct_etiqueta_opcao_tb");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.Property(a => a.Descricao)
                .HasColumnName("descricao");
        }
    }
}
