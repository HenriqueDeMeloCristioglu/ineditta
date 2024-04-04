using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosDescontosPagamentosVencimentos
{
    internal sealed class EventoCalendarioDescontoPagamentoVencimentoVwMap : IEntityTypeConfiguration<EventoCalendarioDescontoPagamentoVencimentoVw>
    {
        public void Configure(EntityTypeBuilder<EventoCalendarioDescontoPagamentoVencimentoVw> builder)
        {
            builder.HasNoKey();

            builder.ToView("evento_calendario_desconto_pagamento_vencimento_vw");

            builder.Property(p => p.Data)
                .HasColumnName("data_evento");

            builder.Property(p => p.ClausulaGeralId)
                .HasColumnName("clausula_geral_id");

            builder.Property(p => p.NomeDocumento)
                .HasColumnName("nome_documento");

            builder.Property(p => p.AtividadesEconomicas)
                .HasColumnName("atividades_economicas");

            builder.Property(p => p.SiglasSindicatosPatronais)
                .HasColumnName("sindicatos_patronais");

            builder.Property(p => p.SiglasSindicatosLaborais)
                .HasColumnName("sindicatos_laborais");

            builder.Property(p => p.NomeEvento)
                .HasColumnName("nome_evento");

            builder.Property(p => p.GrupoClausulas)
                .HasColumnName("nome_grupo");

            builder.Property(p => p.NomeClausula)
                .HasColumnName("nome_clausula");

            builder.Property(p => p.Origem)
                .HasColumnName("origem_evento");

            builder.Property(p => p.SindicatoLaboralId)
                .HasColumnName("sindicatos_laborais_ids");

            builder.Property(p => p.SindicatoPatronalId)
                .HasColumnName("sindicatos_patronais_ids");

            builder.Property(p => p.Id)
                .HasColumnName("id_evento");
        }
    }
}
