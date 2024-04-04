using Ineditta.Repository.Clausulas.Geral.Models;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.MapasSindicais.Views
{
    internal sealed class DocumentoMapaSindicalVwMap : IEntityTypeConfiguration<DocumentoMapaSindicalVw>
    {
        public void Configure(EntityTypeBuilder<DocumentoMapaSindicalVw> builder)
        {
            builder.ToView("documento_mapa_sindical_vw");

            builder.HasNoKey();

            builder.Property(e => e.DocumentoId).HasColumnName("documento_id");

            builder.Property(e => e.DocumentoSindicatoPatronal)
               .HasColumnType("json")
               .HasColumnName("documento_sindicato_patronal")
               .HasConversion(new GenericConverter<SindPatronal[]>());

            builder.Property(e => e.DocumentoSindicatoLaboral)
                .HasColumnType("json")
                .HasColumnName("documento_sindicato_laboral")
                .HasConversion(new GenericConverter<SindLaboral[]>());

            builder.Property(e => e.DataRegistro).HasColumnName("data_registro");

            builder.Property(e => e.DocumentoDataAprovacao).HasColumnName("documento_data_aprovacao");

            builder.Property(e => e.ClausulaGeralDataAprovacao).HasColumnName("clausula_geral_data_aprovacao");

            builder.Property(e => e.DocumentoDatabase).HasColumnName("documento_database");

            builder.Property(e => e.DocumentoTipoId).HasColumnName("documento_tipo_id");

            builder.Property(e => e.DocumentoCnae)
                .HasColumnType("json")
                .HasColumnName("documento_cnae")
                .HasConversion(new GenericConverter<CnaeDoc[]>());

            builder.Property(e => e.DocumentoAbrangencia)
                .HasColumnType("json")
                .HasColumnName("documento_abrangencia")
                .HasConversion(new GenericConverter<Abrangencia[]>());

            builder.Property(e => e.DocumentoEstabelecimento)
                .HasColumnType("json")
                .HasColumnName("documento_estabelecimento")
                .HasConversion(new GenericConverter<ClienteEstabelecimento[]>());

            builder.Property(e => e.DocumentoValidadeInicial).HasColumnName("documento_validade_inicial");

            builder.Property(e => e.DocumentoValidadeFinal).HasColumnName("documento_validade_final");

            builder.Property(e => e.DocumentoUf).HasColumnName("documento_uf");

            builder.Property(e => e.DocumentoDescricao).HasColumnName("documento_descricao");

            builder.Property(e => e.DocumentoTitulo).HasColumnName("documento_titulo");

            builder.Property(p => p.TipoDocumentoNome).HasColumnName("tipo_documento_nome");

            builder.Property(p => p.DataLiberacaoClausulas).HasColumnName("data_liberacao_clausulas");
        }
    }
}
