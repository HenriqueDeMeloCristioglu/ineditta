using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosDocumentos
{
    internal sealed class EventoCalendarioVencimentoDocumentoVwMap : IEntityTypeConfiguration<EventoCalendarioVencimentoDocumentoVw>
    {
        public void Configure(EntityTypeBuilder<EventoCalendarioVencimentoDocumentoVw> builder)
        {
            builder.HasNoKey();

            builder.ToView("evento_calendario_vencimento_documento_vw");

            builder.Property(p => p.TipoEvento)
                .HasColumnName("tipo_evento");

            builder.Property(p => p.SindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("sind_patronal")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoPatronal>?>());

            builder.Property(p => p.SindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("sind_laboral")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoLaboral>?>());

            builder.Property(p => p.Data)
                .HasColumnName("data");

            builder.Property(p => p.Origem)
                .HasColumnName("origem");

            builder.Property(p => p.AtividadesEconomicas)
                .HasColumnName("atividades_economicas");

            builder.Property(p => p.ValidadeInicial)
                .HasColumnName("validade_inicial");

            builder.Property(p => p.ValidadeFinal)
                .HasColumnName("validade_final");

            builder.Property(p => p.TipoDocId)
                .HasColumnName("tipo_doc_id");

            builder.Property(p => p.TipoDocNome)
                .HasColumnName("tipo_doc_nome");

            builder.Property(p => p.ChaveReferenciaId)
                .HasColumnName("chave_referencia");
        }
    }
}
