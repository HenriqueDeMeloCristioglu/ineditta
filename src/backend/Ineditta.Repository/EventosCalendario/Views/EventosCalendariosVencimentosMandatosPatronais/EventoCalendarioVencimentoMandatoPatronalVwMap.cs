using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosMandatosPatronais
{
    internal sealed class EventoCalendarioVencimentoMandatoPatronalVwMap : IEntityTypeConfiguration<EventoCalendarioVencimentoMandatoPatronalVw>
    {
        public void Configure(EntityTypeBuilder<EventoCalendarioVencimentoMandatoPatronalVw> builder)
        {
            builder.HasNoKey();

            builder.ToView("evento_calendario_vencimento_mandato_patronal_vw");

            builder.Property(p => p.SiglasSindicatosPatronais)
                .HasColumnName("siglas_sindicatos_patronais");

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

            builder.Property(p => p.Dirigente)
                .HasColumnName("dirigente");

            builder.Property(p => p.Funcao)
                .HasColumnName("funcao");

            builder.Property(p => p.SindId)
                .HasColumnName("sindicato_id");
        }
    }
}
