using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosAssembleiasReunioes
{
    public class EventoCalendariaAssembleiaReuniaoVwMap : IEntityTypeConfiguration<EventoCalendarioAssembleiaReuniaoVw>
    {
        public void Configure(EntityTypeBuilder<EventoCalendarioAssembleiaReuniaoVw> builder)
        {
            builder.HasNoKey();

            builder.ToView("calendario_sindical_assembleia_reuniao_vw");

            builder.Property(e => e.Data)
                .HasColumnName("data_referencia");

            builder.Property(e => e.Origem)
                .HasColumnName("origem");

            builder.Property(e => e.TipoEvento)
                .HasColumnName("tipo_evento");

            builder.Property(e => e.SindicatosLaborais)
                .HasConversion(new GenericConverter<IEnumerable<SindicatoEvento>?>())
                .HasColumnName("sindicatos_laborais");

            builder.Property(e => e.SindicatosPatronais)
                .HasConversion(new GenericConverter<IEnumerable<SindicatoEvento>?>())
                .HasColumnName("sindicatos_patronais");

            builder.Property(e => e.AtividadesEconomicas)
                .HasColumnType("json")
                .HasColumnName("atividades_economicas_ids");

            builder.Property(e => e.DataBaseNegociacao)
                .HasColumnName("data_base");

            builder.Property(e => e.TipoDocId)
                .HasColumnName("tipo_doc_id");

            builder.Property(e => e.TipoDocNome)
                .HasColumnName("nome_documento");

            builder.Property(e => e.Fase)
                .HasColumnName("fase_documento");

            builder.Property(e => e.ChaveReferenciaId)
                .HasColumnName("chave_referencia");

            builder.Property(e => e.DescricoesSubclasse)
                .HasColumnName("descricoes_subclasses");

            builder.Property(e => e.TipoEventoId)
                .HasColumnName("tipo_evento_id");
        }
    }
}
