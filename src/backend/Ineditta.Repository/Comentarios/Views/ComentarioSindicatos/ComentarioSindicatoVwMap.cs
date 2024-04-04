using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Comentarios.Views
{
    public class ComentarioSindicatoVwMap : IEntityTypeConfiguration<ComentarioSindicatoVw>
    {
        public void Configure(EntityTypeBuilder<ComentarioSindicatoVw> builder)
        {
            builder.ToView("comentario_sindicato_vw");

            builder.HasNoKey();

            builder.Property(e => e.Comentario).HasColumnName("notificacao_comentario");

            builder.Property(e => e.SindicatoId).HasColumnName("sindicato_id");

            builder.Property(e => e.SindicatoSigla).HasColumnName("sindicato_sigla");

            builder.Property(e => e.SindicatoRazaoSocial).HasColumnName("sindicato_razao_social");

            builder.Property(e => e.SindicatoTipo).HasColumnName("tipo");

            builder.Property(e => e.ComentarioEtiqueta).HasColumnName("notificacao_etiqueta");

            builder.Property(e => e.ComentarioResponsavelId).HasColumnName("usuario_publicacao_id");

            builder.Property(e => e.ComentarioResponsavelNome).HasColumnName("usuario_publicacao");

            builder.Property(e => e.ComentarioDataRegistro).HasColumnName("notificacao_data_registro");

            builder.Property(e => e.ComentarioResponsavelGrupoEconomicoId).HasColumnName("usuario_publicacao_grupo_economico_id");

            builder.Property(e => e.ComentarioResponsavelNivel).HasColumnName("usuario_publicacao_tipo");

            builder.Property(e => e.SindicatoUf).HasColumnName("sindicato_localizacao_uf");

            builder.Property(e => e.ComentarioTipoDestino).HasColumnName("notificacao_tipo_destino");

            builder.Property(e => e.ComentarioDestinoId).HasColumnName("notificacao_tipo_destino_id");

            builder.Property(e => e.ComentarioEstabelecimentoDestinoId).HasColumnName("estabelecimento_id");

            builder.Property(e => e.ComentarioEmpresaDestinoId).HasColumnName("empresa_id");

            builder.Property(e => e.Visivel).HasConversion(new BooleanToIntConverter()).HasColumnName("visivel");

            builder.Property(e => e.ComentarioEmpresaGrupoEconomicoDestinoId).HasColumnName("grupo_economico_id");


        }
    }
}
