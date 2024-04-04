using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisVw
{
    internal sealed class DocumentoSindicalVwMap : IEntityTypeConfiguration<DocumentoSindicalVw>
    {
        public void Configure(EntityTypeBuilder<DocumentoSindicalVw> builder)
        {
            builder.ToView("documento_sindical_vw");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id_doc");

            builder.Property(p => p.Nome)
                .HasColumnName("nome_doc");

            builder.Property(p => p.DataUpload)
                .HasColumnName("data_upload");

            builder.Property(p => p.DataValidadeInicial)
                .HasColumnName("validade_inicial");

            builder.Property(p => p.DataValidadeFinal)
                .HasColumnName("validade_final");

            builder.Property(p => p.Modulo)
                .HasColumnName("modulo");

            builder.Property(p => p.DataBase)
                .HasColumnName("database_doc");

            builder.Property(p => p.DataLiberacaoClausulas)
                .HasColumnName("data_liberacao_clausulas");

            builder.Property(p => p.Arquivo)
                .HasColumnName("caminho_arquivo");

            builder.Property(p => p.Descricao)
                .HasColumnName("descricao_documento");

            builder.Property(p => p.SindicatosLaboraisSiglas)
                .HasColumnName("sindicatos_laborais_siglas")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<string>>());

            builder.Property(p => p.SindicatosLaboraisSiglasString)
                .HasColumnName("siglas_laborais");

            builder.Property(p => p.SindicatosPatronaisSiglas)
                .HasColumnName("sindicatos_patronais_siglas")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<string>>());

            builder.Property(p => p.SindicatosPatronaisSiglasString)
                .HasColumnName("siglas_patronais");

            builder.Property(p => p.SindicatosLaboraisMunicipios)
                .HasColumnName("sindicatos_laborais_municipios")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<string>>());

            builder.Property(p => p.Assuntos)
                .HasColumnName("assuntos")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<string>>());

            builder.Property(p => p.AssuntosStringForSearch)
                .HasColumnName("assuntos_string_for_search");

            builder.Property(p => p.TipoDocumento)
                .HasColumnName("tipo_documento");

            builder.Property(p => p.Restrito)
                .HasColumnName("doc_restrito");

            builder.Property(p => p.Anuencia)
                .HasColumnName("anuencia");

            builder.Property(p => p.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder.Property(p => p.SiglaDoc)
                .HasColumnName("sigla_doc");

            builder.Property(p => p.UsuarioCadastro)
                .HasColumnName("usuario_cadastro");

            builder.Property(p => p.TipoDocId)
                .HasColumnName("tipo_doc_id");

            builder.Property(p => p.AtividadesEconomicas)
                .HasColumnName("cnae_doc")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<object>>());

            builder.Property(p => p.Observacao)
                .HasColumnName("observacao");

            builder.Property(p => p.SindicatoPatronal)
                .HasColumnName("sind_patronal")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<object>>());

            builder.Property(p => p.SindicatoLaboral)
                .HasColumnName("sind_laboral")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<object>>());

            builder.Property(p => p.Abrangencias)
                .HasColumnName("abrangencia")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<object>>());

            builder.Property(p => p.SindicatosLaboraisIds)
                .HasColumnName("sindicatos_laborais_ids")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<int>>());

            builder.Property(p => p.SindicatosPatronaisIds)
                .HasColumnName("sindicatos_patronais_ids")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<int>>());

            builder.Property(p => p.DataInclusao)
                .HasColumnName("data_inclusao");

            builder.Property(p => p.Estabelecimentos)
                .HasColumnName("cliente_estabelecimento")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<IEnumerable<Estabelecimento>>());

            builder.Property(p => p.AbrangenciaUsuario)
                .HasColumnName("abrangencia_usuario");
        }
    }
}
