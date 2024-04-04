using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.AcompanhamentosCct.Views.AcompanhamentosCctsRelatorios
{
    public sealed class AcompanhamentoCctRelatorioVwMap : IEntityTypeConfiguration<AcompanhamentoCctRelatorioVw>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctRelatorioVw> builder)
        {
            builder.ToView("acompanhamento_cct_relatorio_vw");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.NomeDocumento).HasColumnName("nome_documento");

            builder.Property(e => e.DataBase).HasColumnName("data_base");

            builder.Property(e => e.Fase).HasColumnName("fase");

            builder.Property(e => e.FaseId).HasColumnName("fase_id");

            builder.Property(e => e.ObservacoesGerais).HasColumnName("observacoes_gerais");

            builder.Property(e => e.DataProcessamento).HasColumnName("data_processamento");

            builder.Property(e => e.PeriodoAnterior).HasColumnName("periodo_anterior");

            builder.Property(e => e.Indicador).HasColumnName("indicador");

            builder.Property(e => e.DadoReal).HasColumnName("dado_real");

            builder.Property(e => e.AtividadesEconomicas).HasColumnName("atividades_economicas");

            builder.Property(e => e.IrPeriodo).HasColumnName("ir_periodo");

            builder.Property(e => e.Estabelecimentos)
                .HasColumnType("json")
                .HasColumnName("estabelecimentos")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<EstatabelecimentoRelatorio>?>());

            builder.Property(e => e.SindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("sindicatos_laborais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoRelatorio>?>());

            builder.Property(e => e.SindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("sindicatos_patronais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoRelatorio>?>());

            builder.Property(e => e.Ufs)
                .HasColumnType("json")
                .HasColumnName("ufs")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<string>?>());

            builder.Property(e => e.Municipios)
                .HasColumnType("json")
                .HasColumnName("municipios")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<string>?>());
        }
    }
}
