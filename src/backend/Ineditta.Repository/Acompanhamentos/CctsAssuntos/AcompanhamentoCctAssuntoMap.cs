using Ineditta.Application.Acompanhamentos.CctsAssuntos.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsAssuntos
{
    public class AcompanhamentoCctAssuntoMap : IEntityTypeConfiguration<AcompanhamentoCctAssunto>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctAssunto> builder)
        {
            builder.ToTable("acompanhamento_cct_assunto_tb");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
            .HasColumnName("id");

            builder.Property("AcompanhamentoCctId")
                .HasColumnName("acompanhamento_cct_id");

            builder.Property(a => a.EstrutucaClausulaId)
                .HasColumnName("estrutura_clausula_id");

            builder.HasOne<EstruturaClausula>()
                .WithMany()
                .HasForeignKey(a => a.EstrutucaClausulaId)
                .HasConstraintName("fk_acompanhamento_cct_assunto_x_estrutura_clausula");
        }
    }
}
