using Ineditta.Application.Cnaes.Entities;
using Ineditta.Application.Documentos.AtividadesEconomicas.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.AtividadesEconomicas
{
    internal sealed class DocumentoAtividadeEconomicaMap : IEntityTypeConfiguration<DocumentoAtividadeEconomica>
    {
        public void Configure(EntityTypeBuilder<DocumentoAtividadeEconomica> builder)
        {
            builder.ToTable("documento_atividade_economica_tb");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("id");

            builder.Property(d => d.DocumentoId)
                .HasColumnName("documento_id");

            builder.Property(d => d.AtividadeEconomicaId)
                .HasColumnName("atividade_economica_id");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(d => d.DocumentoId)
                .HasConstraintName("fk_documento_atividade_economica_x_doc_sind");

            builder.HasOne<Cnae>()
                .WithMany()
                .HasForeignKey(d => d.AtividadeEconomicaId)
                .HasConstraintName("fk_documento_atividade_economica_x_classes_cnae");
        }
    }
}
