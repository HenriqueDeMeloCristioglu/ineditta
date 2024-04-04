using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.SubtiposEventosCalendarioSindical.Entities;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EventosCalendario
{
    internal sealed class EventoMap : IEntityTypeConfiguration<CalendarioSindical>
    {
        public void Configure(EntityTypeBuilder<CalendarioSindical> builder)
        {
            builder.ToTable("calendario_sindical_tb");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(e => e.NotificarAntes)
                .HasColumnName("notificar_antes")
                .HasDefaultValueSql("00:00:00");

            builder.Property(e => e.Origem)
                .HasColumnName("origem");

            builder.Property(e => e.TipoEvento)
                .HasColumnName("tipo_evento");

            builder.Property(e => e.SubtipoEvento)
                .HasColumnName("subtipo_evento");

            builder.Property(e => e.ChaveReferenciaId)
                .HasColumnName("chave_referencia");

            builder.Property<DateTime>("DataInclusao")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_inclusao");

            builder.Property(e => e.DataReferencia)
                .HasColumnName("data_referencia");

            builder.Property(e => e.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true);

            builder.HasOne<TipoEventoCalendarioSindical>()
                .WithMany()
                .HasForeignKey(e => e.TipoEvento)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("calendario_sindical_tb_ibfk_1");

            builder.HasOne<SubtipoEventoCalendarioSindical>()
                .WithMany()
                .HasForeignKey(e => e.SubtipoEvento)
                .HasConstraintName("calendario_sindical_tb_ibfk_2");
        }
    }
}
