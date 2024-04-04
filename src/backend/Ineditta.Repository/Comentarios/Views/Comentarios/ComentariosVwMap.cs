using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Comentarios.Views.Comentarios
{
    internal sealed class ComentariosVwMap : IEntityTypeConfiguration<ComentariosVw>
    {
        public void Configure(EntityTypeBuilder<ComentariosVw> builder)
        {
            builder.ToView("comentarios_vw");

            builder.HasNoKey();

            builder.Property(c => c.Id)
                .HasColumnName("id");

            builder.Property(c => c.Tipo)
                .HasColumnName("tipo");

            builder.Property(c => c.TipoUsuarioDestino)
                .HasColumnName("tipo_usuario_destino");

            builder.Property(c => c.TipoNotificacao)
                .HasColumnName("tipo_notificacao");

            builder.Property(c => c.DataValidade)
                .HasColumnName("data_validade");

            builder.Property(c => c.NomeUsuario)
                .HasColumnName("nome_usuario");

            builder.Property(c => c.UsuarioId)
                .HasColumnName("usuario_id");

            builder.Property(c => c.EtiquetaId)
                .HasColumnName("etiqueta_id");

            builder.Property(c => c.EtiquetaNome)
                .HasColumnName("etiqueta_nome");

            builder.Property(c => c.Comentario)
                .HasColumnName("comentario");

            builder.Property(c => c.NomeClausula)
                .HasColumnName("nome_clausula");

            builder.Property(c => c.SiglaSindicatoLaboral)
                .HasColumnName("sigla_sinde");

            builder.Property(c => c.SiglaSindicatoPatronal)
                .HasColumnName("sigla_sp");

            builder.Property(c => c.Visivel)
                .HasColumnName("visivel")
                .HasConversion(new BooleanToIntConverter());
        }
    }
}
