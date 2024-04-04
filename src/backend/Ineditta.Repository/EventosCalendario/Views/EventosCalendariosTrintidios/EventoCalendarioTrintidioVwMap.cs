using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Org.BouncyCastle.Pkix;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosTrintidios
{
    internal sealed class EventoCalendarioTrintidioVwMap : IEntityTypeConfiguration<EventoCalendarioTrintidioVw>
    {
        public void Configure(EntityTypeBuilder<EventoCalendarioTrintidioVw> builder)
        {
            builder.HasNoKey();

            builder.ToView("evento_calendario_trintidio_vw");

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

            builder.Property(p => p.SiglasSindicatosPatronais)
                .HasColumnName("siglas_sindicatos_patronais");

            builder.Property(p => p.DataBase)
                .HasColumnName("database_doc");

            builder.Property(p => p.NomeDocumento)
                .HasColumnName("nome_doc");

            builder.Property(p => p.SindicatoLaboralId)
                .HasColumnName("sindicatos_laborais_ids");

            builder.Property(p => p.SindicatoPatronalId)
                .HasColumnName("sindicatos_patronais_ids");

            builder.Property(p => p.ChaveReferenciaId)
                .HasColumnName("chave_referencia");

            builder.Property(p => p.SindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("sind_patronal")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoPatronal>?>());

            builder.Property(p => p.SindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("sind_laboral")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoLaboral>?>());
        }
    }
}
