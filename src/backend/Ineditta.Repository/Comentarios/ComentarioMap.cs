using Ineditta.Application.Comentarios.Entities;
using Ineditta.Application.Etiquetas.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Comentarios
{
    public sealed class ComentarioMap : IEntityTypeConfiguration<Comentario>
    {
        public void Configure(EntityTypeBuilder<Comentario> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");
            builder.ToTable("comentarios_tb");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Valor)
                .HasColumnType("text")
                .HasColumnName("valor")
                .IsRequired();

            builder.Property(e => e.TipoNotificacao)
                .HasColumnType("int")
                .HasColumnName("tipo_notificacao");

            builder.Property(e => e.DataValidade)
                .HasColumnName("data_validade");

            builder.Property(e => e.EtiquetaId)
                .HasMaxLength(70)
                .HasColumnName("etiqueta_id");

            builder.Property(e => e.Tipo)
                .HasColumnType("int")
                .HasColumnName("tipo")
                .IsRequired();

            builder.Property(e => e.ReferenciaId)
                .HasColumnType("int")
                .HasColumnName("referencia_id")
                .IsRequired();

            builder.Property(e => e.TipoUsuarioDestino)
                .HasColumnName("tipo_usuario_destino")
                .HasColumnType("int");

            builder.Property(e => e.UsuarioDestionoId)
                .HasColumnName("usuario_destino_id")
                .HasColumnType("int");

            builder.Property(e => e.Visivel)
                .HasColumnName("visivel")
                .HasConversion(new BooleanToIntConverter());

            builder.HasOne<Etiqueta>()
                .WithMany()
                .HasForeignKey(e => e.EtiquetaId)
                .HasConstraintName("fk_etiqueta_x_comentario");
        }
    }
}
