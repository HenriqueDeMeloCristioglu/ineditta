using Ineditta.Repository.Clausulas.Geral.Models;
using Ineditta.Repository.Converters;
using Ineditta.Repository.InformacoesAdicionais;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Clausulas.Views.InformacoesAdicionais
{
    internal sealed class ClausulaGeralInformacaoAdicionalVwMap : IEntityTypeConfiguration<ClausulaGeralInformacaoAdicionalVw>
    {
        public void Configure(EntityTypeBuilder<ClausulaGeralInformacaoAdicionalVw> builder)
        {
            builder.ToView("clausula_geral_info_adicional_vw");

            builder.HasNoKey();

            builder.Property(e => e.ClausulaId).HasColumnName("clausula_id");
            builder.Property(e => e.ClausulaGeralEstruturaId).HasColumnName("clausula_geral_estrutura_id");
            builder.Property(e => e.DocumentoSindicalId).HasColumnName("documento_sindical_id");
            builder.Property(e => e.GrupoClausulaId).HasColumnName("grupo_clausula_id");
            builder.Property(e => e.GrupoClausulaNome).HasColumnName("grupo_clausula_nome");
            builder.Property(e => e.InformacaoAdicionalId).HasColumnName("informacao_adicional_id");
            builder.Property(e => e.InformacaoAdicionalNome).HasColumnName("informacao_adicional_nome");
            builder.Property(e => e.ValorData).HasColumnName("valor_data");
            builder.Property(e => e.ValorNumerico).HasColumnName("valor_numerico");
            builder.Property(e => e.ValorTexto).HasColumnName("valor_texto");
            builder.Property(e => e.ValorPercentual).HasColumnName("valor_percentual");
            builder.Property(e => e.ValorDescricao).HasColumnName("valor_descricao");
            builder.Property(e => e.ValorHora).HasColumnName("valor_hora");
            builder.Property(e => e.ValorCombo).HasColumnName("valor_combo");
            builder.Property(e => e.Sequencia).HasColumnName("sequencia");
            builder.Property(e => e.GrupoDados).HasColumnName("grupo_dados");
            builder.Property(e => e.DocumentoTitulo).HasColumnName("documento_titulo");
            builder.Property(e => e.DocumentoDataAprovacao).HasColumnName("documento_data_aprovacao");
            builder.Property(e => e.DocumentoValidadeInicial).HasColumnName("documento_validade_inicial");
            builder.Property(e => e.DocumentoValidadeFinal).HasColumnName("documento_validade_final");
            builder.Property(e => e.DataBase).HasColumnName("documento_database");
            builder.Property(e => e.EstruturaClausulaTipo).HasColumnName("estrutura_clausula_tipo");
            builder.Property(e => e.InformacaoAdicionalTipoDado)
                .HasColumnName("informacao_adicional_tipo_dado")
                .HasConversion(new TipoDadoConverter());

            builder.Property(e => e.ClausulaAprovada).HasColumnName("clausula_aprovada");
            builder.Property(e => e.ClausulaDataAprovacao).HasColumnName("clausula_data_aprovacao");
            builder.Property(e => e.ClausulaTexto).HasColumnName("clausula_texto");
            builder.Property(e => e.ClausulaLiberada).HasColumnName("clausula_liberada");

            builder.Property(e => e.DocumentoSindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("documento_sindicatos_laborais")
                .HasConversion(new GenericConverter<SindLaboral[]?>());

            builder.Property(e => e.DocumentoSindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("documento_sindicatos_patronais")
                .HasConversion(new GenericConverter<SindPatronal[]?>());

            builder.Property(e => e.AtividadeEconomicas)
                .HasColumnType("json")
                .HasColumnName("documento_atividades_economicas")
                .HasConversion(new GenericConverter<CnaeDoc[]?>());

            builder.Property(e => e.Abrangencias)
                .HasColumnType("json")
                .HasColumnName("documento_abrangencia")
                .HasConversion(new GenericConverter<Abrangencia[]?>());


            builder.Property(p => p.DocumentoUf)
                .HasColumnName("documento_uf");

            builder.Property(p => p.TipoDocumentoNome)
                .HasColumnName("tipo_documento_nome");

            builder.Property(p => p.EstruturaClausulaId)
                .HasColumnName("estrutura_clausula_id");

            builder.Property(p => p.CodigosUnidades)
                .HasColumnName("codigos_unidades");

            builder.Property(p => p.CnpjUnidades)
                .HasColumnName("cnpjs_unidades");

            builder.Property(p => p.CodigosSindicatoClienteUnidades)
                .HasColumnName("codigos_sindicato_cliente_unidades");

            builder.Property(p => p.DenominacoesPatronais)
                .HasColumnName("denominacoes_patronais");

            builder.Property(p => p.DenominacoesLaborais)
                .HasColumnName("denominacoes_laborais");

            builder.Property(p => p.UfsUnidades)
                .HasColumnName("ufs_unidades");

            builder.Property(p => p.MunicipiosUnidades)
                .HasColumnName("municipios_unidades");
        }
    }
}
