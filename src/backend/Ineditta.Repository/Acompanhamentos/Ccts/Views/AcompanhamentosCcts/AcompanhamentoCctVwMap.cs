using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCcts
{
    public class AcompanhamentoCctVwMap : IEntityTypeConfiguration<AcompanhamentoCctVw>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctVw> builder)
        {
            builder.ToView("acompanhamento_cct_vw");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.SindicatosLaboraisCodigos).HasColumnName("sindicatos_laborais_codigos");

            builder.Property(e => e.SindicatosLaboraisCnpjs).HasColumnName("sindicatos_laborais_cnpjs");

            builder.Property(e => e.SindicatosLaboraisIds).HasColumnName("sindicatos_laborais_ids");

            builder.Property(e => e.SindicatosLaboraisUfs).HasColumnName("sindicatos_laborais_ufs");

            builder.Property(e => e.SindicatosLaboraisSiglas).HasColumnName("sindicatos_laborais_siglas");

            builder.Property(e => e.SindicatosPatronaisCodigos).HasColumnName("sindicatos_patronais_codigos");

            builder.Property(e => e.SindicatosPatronaisCnpjs).HasColumnName("sindicatos_patronais_cnpjs");

            builder.Property(e => e.SindicatosPatronaisIds).HasColumnName("sindicatos_patronais_ids");

            builder.Property(e => e.SindicatosPatronaisUfs).HasColumnName("sindicatos_patronais_ufs");

            builder.Property(e => e.SindicatosPatronaisSiglas).HasColumnName("sindicatos_patronais_siglas");

            builder.Property(e => e.NomeDocumento).HasColumnName("nome_documento");

            builder.Property(e => e.DataBase).HasColumnName("data_base");

            builder.Property(e => e.Fase).HasColumnName("fase");

            builder.Property(e => e.FaseId).HasColumnName("fase_id");

            builder.Property(e => e.ObservacoesGerais).HasColumnName("observacoes_gerais");

            builder.Property(e => e.IdsCnaes)
                .HasConversion(new GenericConverter<IEnumerable<string>>())
                .HasColumnName("ids_cnaes");

            builder.Property(e => e.PeriodoAnterior).HasColumnName("periodo_anterior");

            builder.Property(e => e.Indicador).HasColumnName("indicador");

            builder.Property(e => e.DadoReal).HasColumnName("dado_real");

            builder.Property(e => e.AtividadesEconomicas).HasColumnName("atividades_economicas");

            builder.Property(e => e.IrPeriodo).HasColumnName("ir_periodo");

            builder.Property(e => e.DataProcessamento).HasColumnName("data_processamento");
        }
    }
}
