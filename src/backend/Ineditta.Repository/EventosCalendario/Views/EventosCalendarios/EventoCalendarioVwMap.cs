using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendarios
{
    internal sealed class EventoCalendarioVwMap : IEntityTypeConfiguration<EventoCalendarioVw>
    {
        public void Configure(EntityTypeBuilder<EventoCalendarioVw> builder)
        {
            builder.HasNoKey();

            builder.ToView("evento_calendario_vw");

            builder.Property(p => p.SiglasSindicatosPatronais)
                .HasColumnName("siglas_sindicatos_patronais");

            builder.Property(p => p.SiglasSindicatosLaborais)
                .HasColumnName("siglas_sindicatos_laborais");

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
                .HasColumnName("tipo_doc");
        }
    }
}
