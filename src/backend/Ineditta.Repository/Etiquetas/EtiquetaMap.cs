using Ineditta.Application.Etiquetas.Entities;
using Ineditta.Application.TiposEtiquetas.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Etiquetas
{
    public sealed class EtiquetaMap : IEntityTypeConfiguration<Etiqueta>
    {
        public void Configure(EntityTypeBuilder<Etiqueta> builder)
        {
            builder.ToTable("etiqueta_tb");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Nome)
                .HasColumnType("text")
                .HasColumnName("nome");

            builder.Property(e => e.TipoEtiquetaId)
                .HasColumnName("tipo_etiqueta_id");

            builder.HasOne<TipoEtiqueta>()
                .WithMany()
                .HasForeignKey(e => e.TipoEtiquetaId)
                .HasConstraintName("fk_etiqueta_x_tipo_etiqueta");
        }
    }
}
