using Ineditta.Application.SubtiposEventosCalendarioSindical.Entities;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.SubtiposEventosCalendarioSindical
{
    public sealed class SubtipoEventoCalendarioSindicalMap : IEntityTypeConfiguration<SubtipoEventoCalendarioSindical>
    {
        public void Configure(EntityTypeBuilder<SubtipoEventoCalendarioSindical> builder)
        {
            builder.ToTable("subtipo_evento_calendario_sindical");
            builder.HasKey(p => p.Id).HasName("PRIMARY");

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");

            builder.Property(p => p.TipoEventoId)
                .HasColumnName("tipo_evento_id");

            builder.HasOne<TipoEventoCalendarioSindical>()
                .WithMany()
                .HasForeignKey(p => p.TipoEventoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("subtipo_evento_ibfk_1");
        }
    }
}
