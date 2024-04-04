using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisClausulasVw
{
    public class DocumentoSindicalClausulasVwMap : IEntityTypeConfiguration<DocumentoSindicalClausulasVw>
    {
        public void Configure(EntityTypeBuilder<DocumentoSindicalClausulasVw> builder)
        {

            builder.ToView("documento_sindical_clausulas_vw");

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

            builder.Property(p => p.Arquivo)
                .HasColumnName("caminho_arquivo");

            builder.Property(p => p.Descricao)
                .HasColumnName("descricao_documento");

            builder.Property(p => p.SindicatosLaboraisSiglas)
                .HasColumnName("sindicatos_laborais_siglas")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<string>>());

            builder.Property(p => p.SindicatosPatronaisSiglas)
                .HasColumnName("sindicatos_patronais_siglas")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<string>>());

            builder.Property(p => p.SindicatosLaboraisMunicipios)
                .HasColumnName("sindicatos_laborais_municipios")
                .HasColumnType("json")
                .HasConversion(new GenericConverter<List<string>>());

            builder.Property(p => p.TipoDocumento)
                .HasColumnName("tipo_documento");

            builder.Property(p => p.Restrito)
                .HasColumnName("doc_restrito");

            builder.Property(p => p.Anuencia)
                .HasColumnName("anuencia");

            builder.Property(p => p.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder.Property(p => p.ClausulaQuantidadeNaoAprovadas)
                .HasColumnName("clausula_quantidade_nao_aprovadas");

            builder.Property(p => p.ClausulaDataUltimaAprovacao)
                .HasColumnName("clausula_data_ultima_aprovacao");

            builder.Property(p => p.SiglaDoc)
                .HasColumnName("sigla_doc");
        }
    }
}
