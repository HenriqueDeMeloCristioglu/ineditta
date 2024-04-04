using Ineditta.Application.DocumentosLocalizados.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.Localizados
{
    internal sealed class DocumentoLocalizadoMap : IEntityTypeConfiguration<DocumentoLocalizado>
    {
        public void Configure(EntityTypeBuilder<DocumentoLocalizado> builder)
        {
            builder.ToTable("documentos_localizados");

            builder.HasKey(p => p.Id).HasName("PRIMARY");

            builder
                .Property(p => p.Id)
                .HasColumnName("id_documento");

            builder
                .Property(p => p.CaminhoArquivo)
                .HasMaxLength(255)
                .HasColumnName("caminho");

            builder
                .Property(p => p.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder
                .Property(p => p.DataRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_registro");

            builder
                .Property(p => p.IdLegado)
                .HasColumnName("id_legado");

            builder
                .Property(p => p.NomeDocumento)
                .HasMaxLength(255)
                .HasColumnName("nome_documento");

            builder
                .Property(p => p.Origem)
                .HasMaxLength(45)
                .HasColumnName("origem");

            builder
                .Property(p => p.Referenciado)
                .HasMaxLength(3)
                .HasDefaultValueSql("'não'")
                .HasColumnName("referenciado")
                .HasConversion(new BooleanToStringFullLowerCaseConverter());

            builder.Property(p => p.Situacao)
                .HasMaxLength(45)
                .HasDefaultValueSql("'não aprovado'")
                .HasColumnName("situacao")
                .HasConversion(new EnumToStringConverter<Situacao>());

            builder.Property(e => e.Uf)
                .HasMaxLength(4)
                .HasColumnName("uf");
        }
    }
}
