using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view comentario_sindicato_vw as
                    select se.id_sinde sindicato_id, se.sigla_sinde sindicato_sigla, se.razaosocial_sinde sindicato_razao_social, 'laboral' as tipo,
                    nc.tipo_comentario notificacao_tipo_comentario, nc.comentario notificacao_comentario, nc.id_notecliente notificacao_id,
                    nc.etiqueta notificacao_etiqueta, nc.data_registro notificacao_data_registro, nc.tipo_usuario_destino notificacao_tipo_destino,
                    nc.id_tipo_usuario_destino notificacao_tipo_destino_id, adm.id_user usuario_publicacao_id, adm.nome_usuario usuario_publicacao,
                    adm.id_grupoecon usuario_publicacao_grupo_economico_id, adm.nivel usuario_publicacao_tipo,
                    se.uf_sinde sindicato_localizacao_uf,
                    cut.id_unidade estabelecimento_id, 
                    coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) empresa_id, 
                    coalesce((CASE WHEN nc.tipo_usuario_destino = 'grupo' THEN nc.id_tipo_usuario_destino ELSE NULL END),
    		                 (CASE WHEN nc.tipo_usuario_destino = 'matriz' THEN cmt.cliente_grupo_id_grupo_economico ELSE NULL END),
    		                 cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) grupo_economico_id
                    from sind_emp se
                    left join note_cliente nc on nc.tipo_comentario = 'laboral' and se.id_sinde = nc.id_tipo_comentario
                    left join usuario_adm adm on adm.id_user = nc.usuario_adm_id_user
                    left join cliente_unidades cut on nc.tipo_usuario_destino = 'unidade' and nc.id_tipo_usuario_destino = cut.id_unidade 
                    left join cliente_matriz cmt on nc.tipo_usuario_destino = 'matriz' and nc.id_tipo_usuario_destino = cmt.id_empresa 
                    left join cliente_grupo cgt on nc.tipo_usuario_destino = 'grupo' and nc.id_tipo_usuario_destino = cgt.id_grupo_economico 
                    union all
                    select sp.id_sindp sindicato_id, sp.sigla_sp sindicato_sigla, sp.razaosocial_sp sindicato_razao_social, 'patronal' as tipo,
                    nc.tipo_comentario notificacao_tipo_comentario, nc.comentario notificacao_comentario, nc.id_notecliente notificacao_id,
                    nc.etiqueta notificacao_etiqueta, nc.data_registro notificacao_data_registro, nc.tipo_usuario_destino notificacao_tipo_destino,
                    nc.id_tipo_usuario_destino notificacao_tipo_destino_id, adm.id_user usuario_publicacao_id, adm.nome_usuario usuario_publicacao,
                    adm.id_grupoecon usuario_publicacao_grupo_economico_id, adm.nivel usuario_publicacao_tipo,
                    sp.uf_sp sindicato_localizacao_uf,
                    cut.id_unidade estabelecimento_id, 
                    coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) empresa_id, 
                    coalesce((CASE WHEN nc.tipo_usuario_destino = 'grupo' THEN nc.id_tipo_usuario_destino ELSE NULL END),
    		                 (CASE WHEN nc.tipo_usuario_destino = 'matriz' THEN cmt.cliente_grupo_id_grupo_economico ELSE NULL END),
    		                 cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) grupo_economico_id
                    from sind_patr sp
                    left join note_cliente nc on nc.tipo_comentario = 'patronal' and sp.id_sindp = nc.id_tipo_comentario
                    left join usuario_adm adm on adm.id_user = nc.usuario_adm_id_user
                    left join cliente_unidades cut on nc.tipo_usuario_destino = 'unidade' and nc.id_tipo_usuario_destino = cut.id_unidade 
                    left join cliente_matriz cmt on nc.tipo_usuario_destino = 'matriz' and nc.id_tipo_usuario_destino = cmt.id_empresa 
                    left join cliente_grupo cgt on nc.tipo_usuario_destino = 'grupo' and nc.id_tipo_usuario_destino = cgt.id_grupo_economico 
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view comentario_sindicato_vw as
                    select se.id_sinde sindicato_id, se.sigla_sinde sindicato_sigla, se.razaosocial_sinde sindicato_razao_social, 'laboral' as tipo,
                    nc.tipo_comentario notificacao_tipo_comentario, nc.comentario notificacao_comentario, nc.id_notecliente notificacao_id,
                    nc.etiqueta notificacao_etiqueta, nc.data_registro notificacao_data_registro, nc.tipo_usuario_destino notificacao_tipo_destino,
                    nc.id_tipo_usuario_destino notificacao_tipo_destino_id, adm.id_user usuario_publicacao_id, adm.nome_usuario usuario_publicacao,
                    adm.id_grupoecon usuario_publicacao_grupo_economico_id, adm.nivel usuario_publicacao_tipo,
                    se.uf_sinde sindicato_localizacao_uf,
                    cut.id_unidade estabelecimento_id, coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) empresa_id, COALESCE(cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) grupo_economico_id
                    from sind_emp se
                    left join note_cliente nc on nc.tipo_comentario = 'laboral' and se.id_sinde = nc.id_tipo_comentario
                    left join usuario_adm adm on adm.id_user = nc.usuario_adm_id_user
                    left join cliente_unidades cut on nc.tipo_usuario_destino = 'unidade' and nc.id_tipo_usuario_destino = cut.id_unidade 
                    left join cliente_matriz cmt on nc.tipo_usuario_destino = 'matriz' and nc.id_tipo_usuario_destino = cmt.id_empresa 
                    left join cliente_grupo cgt on nc.tipo_usuario_destino = 'grupo' and nc.id_tipo_usuario_destino = cgt.id_grupo_economico 
                    union all
                    select sp.id_sindp sindicato_id, sp.sigla_sp sindicato_sigla, sp.razaosocial_sp sindicato_razao_social, 'patronal' as tipo,
                    nc.tipo_comentario notificacao_tipo_comentario, nc.comentario notificacao_comentario, nc.id_notecliente notificacao_id,
                    nc.etiqueta notificacao_etiqueta, nc.data_registro notificacao_data_registro, nc.tipo_usuario_destino notificacao_tipo_destino,
                    nc.id_tipo_usuario_destino notificacao_tipo_destino_id, adm.id_user usuario_publicacao_id, adm.nome_usuario usuario_publicacao,
                    adm.id_grupoecon usuario_publicacao_grupo_economico_id, adm.nivel usuario_publicacao_tipo,
                    sp.uf_sp sindicato_localizacao_uf,
                    cut.id_unidade estabelecimento_id, coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) empresa_id, COALESCE(cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) grupo_economico_id
                    from sind_patr sp
                    left join note_cliente nc on nc.tipo_comentario = 'patronal' and sp.id_sindp = nc.id_tipo_comentario
                    left join usuario_adm adm on adm.id_user = nc.usuario_adm_id_user
                    left join cliente_unidades cut on nc.tipo_usuario_destino = 'unidade' and nc.id_tipo_usuario_destino = cut.id_unidade 
                    left join cliente_matriz cmt on nc.tipo_usuario_destino = 'matriz' and nc.id_tipo_usuario_destino = cmt.id_empresa 
                    left join cliente_grupo cgt on nc.tipo_usuario_destino = 'grupo' and nc.id_tipo_usuario_destino = cgt.id_grupo_economico 
            ");
        }
    }
}
