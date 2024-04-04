using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicatosEstabelecimentosVw
{
    internal sealed class DocumentoSindicatoEstabelecimentoVwMap : IEntityTypeConfiguration<DocumentoSindicatoEstabelecimentoVw>
    {
        public void Configure(EntityTypeBuilder<DocumentoSindicatoEstabelecimentoVw> builder)
        {
            builder.ToView("documento_sindicato_estabelecimentos_vw");

            builder.HasNoKey();

            builder
                .Property(p => p.DocumentoId)
                .HasColumnName("documento_id");

            builder
                .Property(p => p.GrupoEconomicoId)
                .HasColumnName("grupo_economico_id");

            builder
                .Property(p => p.GrupoEconomicoNome)
                .HasColumnName("grupo_economico_nome");

            builder
                .Property(p => p.MatrizId)
                .HasColumnName("matriz_id");

            builder
                .Property(p => p.MatrizNome)
                .HasColumnName("matriz_nome");

            builder
                .Property(p => p.EstabelecimentoId)
                .HasColumnName("estabelecimento_id");

            builder
                .Property(p => p.EstabelecimentoNome)
                .HasColumnName("estabelecimento_nome");

            builder
                .Property(p => p.EstabelecimentoCodigoSindicatoLaboral)
                .HasColumnName("estabelecimento_codigo_sindicato_laboral");

            builder
                .Property(p => p.EstabelecimentoCodigoSindicatoPatronal)
                .HasColumnName("estabelecimento_codigo_sindicato_patronal");

            builder
                .Property(p => p.DocumentoNome)
                .HasColumnName("documento_nome");

            builder
                .Property(p => p.DocumentoTitulo)
                .HasColumnName("documento_titulo");

            builder
                .Property(p => p.DocumentoVersao)
                .HasColumnName("documento_versao");

            builder
                .Property(p => p.DocumentoDatabase)
                .HasColumnName("documento_database");

            builder
                .Property(p => p.DocumentoVigenciaInicial)
                .HasColumnName("documento_vigencia_inicial");

            builder
                .Property(p => p.DocumentoVigenciaFinal)
                .HasColumnName("documento_vigencia_final");

            builder
                .Property(p => p.GrupoEconomicoLogo)
                .HasColumnName("grupo_economico_logo_url");

            builder
                .Property(p => p.EstabelecimentoCodigo)
                .HasColumnName("estabelecimento_codigo");

            builder
                .Property(p => p.DocumentoDataAssinatura)
                .HasColumnName("documento_data_assinatura");

            builder
                .Property(p => p.DocumentoCaminhoArquivo)
                .HasColumnName("documento_caminho_arquivo");

            builder
                .Property(p => p.DocumentoDataRegistroMte)
                .HasColumnName("documento_data_registro_mte");

            builder.Property(e => e.DocumentoSindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("documento_sindicatos_laborais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoLaboral>?>());

            builder.Property(e => e.DocumentoSindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("documento_sindicatos_patronais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoPatronal>?>());

            builder.Property(e => e.EstabelecimentoSindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("estabelecimento_sindicatos_patronais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<EstabelecimentoSindicatoViewModel>?>());

            builder.Property(e => e.EstabelecimentoSindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("estabelecimento_sindicatos_laborais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<EstabelecimentoSindicatoViewModel>?>());
        }
    }
}
