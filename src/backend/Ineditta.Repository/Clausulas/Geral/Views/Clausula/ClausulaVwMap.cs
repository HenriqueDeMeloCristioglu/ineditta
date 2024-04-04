using Ineditta.Repository.Clausulas.Geral.Models;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Clausulas.Geral.Views.Clausula
{
    public class ClausulaVwMap : IEntityTypeConfiguration<ClausulaVw>
    {
        public void Configure(EntityTypeBuilder<ClausulaVw> builder)
        {
            builder.ToView("clausulas_vw");

            builder.HasNoKey();

            builder.Property(e => e.ClausulaId).HasColumnName("clausula_id");

            builder.Property(e => e.ClausulaTexto).HasColumnName("clausula_texto");

            builder.Property(e => e.GrupoClausulaId).HasColumnName("grupo_clausula_id");

            builder.Property(e => e.GrupoClausulaNome).HasColumnName("grupo_clausula_nome");

            builder.Property(e => e.EstruturaClausulaId).HasColumnName("estrutura_clausula_id");

            builder.Property(e => e.EstruturaClausulaNome).HasColumnName("estrutura_clausula_nome");

            builder.Property(e => e.DocumentoId).HasColumnName("documento_id");

            builder.Property(e => e.DocumentoSindicatoLaboral)
                .HasColumnType("json")
                .HasColumnName("documento_sindicato_laboral")
                .HasConversion(new GenericConverter<SindLaboral[]>());

            builder.Property(e => e.DocumentoSindicatoPatronal)
                .HasColumnType("json")
                .HasColumnName("documento_sindicato_patronal")
                .HasConversion(new GenericConverter<SindPatronal[]>());

            builder.Property(e => e.DataRegistro).HasColumnName("data_registro");

            builder.Property(e => e.DataAprovacaoDocumento).HasColumnName("data_aprovacao");

            builder.Property(e => e.DataAprovacaoClausula).HasColumnName("clausula_geral_data_aprovacao");

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
                .HasConversion(new SnakeCaseToPascalCaseConverter<ClienteEstabelecimento[]>());

            builder.Property(e => e.DocumentoValidadeInicial).HasColumnName("documento_validade_inicial");

            builder.Property(e => e.DocumentoValidadeFinal).HasColumnName("documento_validade_final");

            builder.Property(e => e.ClausulaGeralLiberada).HasColumnName("clausula_geral_liberada");

            builder.Property(e => e.QuantidadeSindicatosPatronais).HasColumnName("quantidade_sindicatos_patronais");

            builder.Property(e => e.QuantidadeSindicatosLaborais).HasColumnName("quantidade_sindicatos_laborais");

            builder.Property(e => e.Aprovado).HasColumnName("aprovado");

            builder.Property(e => e.DocumentoReferencia)
                .HasColumnType("json")
                .HasColumnName("documento_referencia")
                .HasConversion(new GenericConverter<string[]>());

            builder.Property(e => e.DocumentoNome).HasColumnName("documento_nome");

            builder.Property(e => e.TextoResumido).HasColumnName("texto_resumido");

            builder.Property(e => e.DataProcessamentoDocumento).HasColumnName("data_processamento_documento");

            builder.Property(e => e.DataAssinaturaDocumento).HasColumnName("data_assinatura_documento");

            builder.Property(e => e.ResumoStatus).HasColumnName("resumo_status");

            builder.Property(e => e.ResumoStatusId).HasColumnName("resumo_status_id");

            builder.Property(e => e.ConstaNoDocumento)
                .HasConversion(new BooleanToIntConverter())
                .HasColumnName("consta_no_documento");

            builder.Property(e => e.Resumivel)
                .HasConversion(new BooleanToIntConverter())
                .HasColumnName("resumivel");

            builder.Property(e => e.Regiao).HasColumnName("regiao");
        }
    }
}
