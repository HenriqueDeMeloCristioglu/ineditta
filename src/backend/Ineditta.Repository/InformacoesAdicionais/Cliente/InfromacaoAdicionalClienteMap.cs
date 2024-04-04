using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.GruposEconomicos.Entities;
using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.Repository.Converters;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.InformacoesAdicionais.Cliente
{
    internal sealed class InfromacaoAdicionalClienteMap : IEntityTypeConfiguration<InformacaoAdicionalCliente>
    {
        public void Configure(EntityTypeBuilder<InformacaoAdicionalCliente> builder)
        {
            builder.ToTable("informacao_adicional_cliente_tb");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.Property(e => e.GrupoEconomicoId).HasColumnName("grupo_economico_id");

            builder.Property(e => e.DocumentoSindicalId).HasColumnName("documento_sindical_id");

            builder.Property(e => e.Aprovado)
                .HasConversion(new BooleanToIntConverter())
                .HasColumnName("aprovado");

            builder.Property(e => e.Orientacao)
                    .HasColumnName("orientacao")
                    .HasColumnType("LONGTEXT");

            builder.Property(e => e.OutrasInformacoes)
                    .HasColumnName("outras_informacoes")
                    .HasColumnType("LONGTEXT");

            builder.Property(e => e.InformacoesAdicionais)
                .HasColumnType("json")
                .HasColumnName("informacoes_adicionais")
                .HasField("_informacoesAdicionais")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<InformacaoAdicional>>());

            builder.Property(e => e.ObservacoesAdicionais)
                .HasColumnType("json")
                .HasColumnName("observacoes_adicionais")
                .HasField("_observacoesAdicionais")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<ObservacaoAdicional>>());

            builder
                .HasOne<GrupoEconomico>()
                .WithMany()
                .HasForeignKey(d => d.GrupoEconomicoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_informacao_adicional_cliente_tb_x_cliente_grupo");

            builder
                .HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(d => d.DocumentoSindicalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_informacao_adicional_cliente_tb_x_docsind");

            builder.HasIndex(p => new { p.DocumentoSindicalId, p.GrupoEconomicoId })
                .IsUnique()
                .HasDatabaseName("uk001_informacao_adicional_cliente_tb");
        }
    }
}
