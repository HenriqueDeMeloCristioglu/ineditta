using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.IA.IADocumentosSindicais.Views.IADocumentosSindicais
{
    internal sealed class IADocumentoSindicalVwMap : IEntityTypeConfiguration<IADocumentoSindicalVw>
    {
        public void Configure(EntityTypeBuilder<IADocumentoSindicalVw> builder)
        {
            builder.ToView("ia_documento_sindical_vw");

            builder.HasNoKey();

            builder.Property(d => d.Id)
                .HasColumnName("id");

            builder.Property(d => d.Nome)
                .HasColumnName("nome");

            builder.Property(d => d.DocumentoReferenciaId)
                .HasColumnName("documento_referencia_id");

            builder.Property(d => d.StatusId)
                .HasColumnType("int")
                .HasColumnName("status");

            builder.Property(d => d.Versao)
                .HasColumnName("versao");

            builder.Property(d => d.DataSla)
                .HasColumnName("data_sla");

            builder.Property(d => d.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder.Property(d => d.UsuarioAprovador)
                .HasColumnName("usuario_aprovador");

            builder.Property(d => d.QuantidadeClausulas)
                .HasColumnName("quantidade_clausulas");

            builder.Property(d => d.StatusGeral)
                .HasColumnName("status_geral");

            builder.Property(d => d.Assunto)
                .HasColumnName("assunto");

            builder.Property(d => d.MotivoErro)
                .HasColumnName("motivo_erro");
        }
    }
}
