using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.SubtiposEventosCalendarioSindical.Entities;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.UsuariosTiposEventosCalendarioSindical
{
    public sealed class UsuarioTipoEventoCalendarioSindicalMap : IEntityTypeConfiguration<UsuarioTipoEventoCalendarioSindical>
    {
        public void Configure(EntityTypeBuilder<UsuarioTipoEventoCalendarioSindical> builder)
        {
            builder.ToTable("usuario_tipo_evento_calendario_sindical");
            builder.HasKey(p => p.Id).HasName("PRIMARY");

            builder.Property(p => p.UsuarioId)
                .HasColumnName("usuario_id");

            builder.Property(p => p.TipoEventoId)
                .HasColumnName("tipo_evento_id");

            builder.Property(p => p.SubtipoEventoId)
                .HasColumnName("subtipo_evento_id");

            builder.Property(p => p.NotificarEmail)
                .HasColumnName("notificar_email");

            builder.Property(p => p.NotificarWhatsapp)
                .HasColumnName("notificar_whatsapp");

            builder.Property(p => p.NotificarAntes)
                .HasDefaultValueSql("'120:00:00'")
                .HasColumnName("notificar_antes");

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usuario_tipo_evento_calendario_sindical_ibfk_1");

            builder.HasOne<TipoEventoCalendarioSindical>()
                .WithMany()
                .HasForeignKey(p => p.TipoEventoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usuario_tipo_evento_calendario_sindical_ibfk_2");

            builder.HasOne<SubtipoEventoCalendarioSindical>()
                .WithMany()
                .HasForeignKey(p => p.SubtipoEventoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usuario_tipo_evento_calendario_sindical_ibfk_3");
        }
    }
}
