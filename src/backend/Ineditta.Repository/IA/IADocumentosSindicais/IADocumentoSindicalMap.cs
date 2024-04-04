using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ineditta.Application.Documentos.Sindicais.Entities;

namespace Ineditta.Repository.IA.IADocumentosSindicais
{
    internal sealed class IADocumentoSindicalMap : IEntityTypeConfiguration<IADocumentoSindical>
    {
        public void Configure(EntityTypeBuilder<IADocumentoSindical> builder)
        {
            builder.ToTable("ia_documento_sindical_tb");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("id");

            builder.Property(d => d.Status)
                .HasColumnName("status");

            builder.Property(d => d.DocumentoReferenciaId)
                .HasColumnName("documento_referencia_id");

            builder.Property(d => d.MotivoErro)
                .HasColumnName("motivo_erro");

            builder.Property(d => d.UltimoStatusProcessado)
                .HasColumnName("ultimo_status_processado");

            builder.Property(d => d.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder.Property(d => d.UsuarioAprovaorId)
                .HasColumnName("usuario_aprovador_id");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(d => d.DocumentoReferenciaId)
                .HasConstraintName("fk_documento_sindical_ia_x_doc_sind");
        }
    }
}
