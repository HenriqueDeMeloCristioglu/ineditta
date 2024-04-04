using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TipoEventoCalendarioSindical = Ineditta.Application.TiposEventosCalendarioSindical.Entities.TipoEventoCalendarioSindical;

namespace Ineditta.Repository.TiposEventosCalendarioSindical
{
    public sealed class TiposEventosCalendarioSindicalMap : IEntityTypeConfiguration<TipoEventoCalendarioSindical>
    {
        public void Configure(EntityTypeBuilder<TipoEventoCalendarioSindical> builder)
        {
            builder.ToTable("tipo_evento_calendario_sindical");
            builder.HasKey(p => p.Id).HasName("PRIMARY");

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
        }
    }
}
