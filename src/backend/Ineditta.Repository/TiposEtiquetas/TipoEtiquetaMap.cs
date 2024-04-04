using Ineditta.Application.TiposEtiquetas.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.TiposEtiquetas
{
    internal sealed class TipoEtiquetaMap : IEntityTypeConfiguration<TipoEtiqueta>
    {
        public void Configure(EntityTypeBuilder<TipoEtiqueta> builder)
        {
            builder.ToTable("tipo_etiqueta_tb");

            builder.HasKey(t => t.Id)
                .HasName("PRIMARY");

            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.Property(t => t.Nome)
                .HasColumnType("text")
                .HasColumnName("nome");
        }
    }
}
