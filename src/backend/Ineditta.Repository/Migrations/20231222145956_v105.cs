using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v105 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "etiqueta",
                table: "comentarios_tb",
                newName: "etiqueta_id");

            migrationBuilder.RenameIndex(
                name: "IX_comentarios_tb_etiqueta",
                table: "comentarios_tb",
                newName: "IX_comentarios_tb_etiqueta_id");

            migrationBuilder.Sql(@"DROP view if exists comentarios_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW comentarios_vw as
                                    SELECT
	                                    id,
                                        CASE ct.tipo
                                            WHEN 1 THEN 'Cláusula'
                                            WHEN 2 THEN 'Sindicato Laboral'
                                            WHEN 3 THEN 'Sindicato Patronal'
                                            WHEN 4 THEN 'Filial'
                                            ELSE ''
                                        END AS tipo,
                                        CASE ct.tipo_usuario_destino
                                            WHEN 1 THEN 'Grupo'
                                            WHEN 2 THEN 'Matriz'
                                            WHEN 3 THEN 'Unidade'
                                            ELSE ''
                                        END AS tipo_usuario_destino,
                                        CASE ct.tipo_notificacao
                                            WHEN 1 THEN 'Fixa'
                                            WHEN 2 THEN 'Temporaria'
                                            ELSE ''
                                        END AS tipo_notificacao,
                                        ct.data_validade,
                                        ua.nome_usuario AS nome_usuario,
                                        ua.id_user AS usuario_id,
                                        ct.valor AS comentario,
                                        ct.etiqueta_id,
                                        ct.visivel
                                    FROM comentarios_tb ct
                                    INNER JOIN usuario_adm ua ON ct.usuario_inclusao_id = ua.id_user;");

            migrationBuilder.Sql(@"DROP view if exists comentario_sindicato_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                ALGORITHM = UNDEFINED VIEW comentario_sindicato_vw AS
                select
                    se.id_sinde AS sindicato_id,
                    se.sigla_sinde AS sindicato_sigla,
                    se.razaosocial_sinde AS sindicato_razao_social,
                    'laboral' AS tipo,
                    CASE ct.tipo
                        WHEN 1 THEN 'Cláusula'
                        WHEN 2 THEN 'Sindicato Laboral'
                        WHEN 3 THEN 'Sindicato Patronal'
                        WHEN 4 THEN 'Filial'
                        ELSE ''
                    END AS notificacao_tipo_comentario,
                    ct.valor AS notificacao_comentario,
                    ct.id AS notificacao_id,
                    et.nome AS notificacao_etiqueta,
                    ct.data_inclusao AS notificacao_data_registro,
                    CASE ct.tipo_usuario_destino
                        WHEN 1 THEN 'Grupo'
                        WHEN 2 THEN 'Matriz'
                        WHEN 3 THEN 'Unidade'
                        ELSE ''
                    END AS notificacao_tipo_destino,
                    ct.usuario_destino_id AS notificacao_tipo_destino_id,
                    ct.visivel,
                    adm.id_user AS usuario_publicacao_id,
                    adm.nome_usuario AS usuario_publicacao,
                    adm.id_grupoecon AS usuario_publicacao_grupo_economico_id,
                    adm.nivel AS usuario_publicacao_tipo,
                    se.uf_sinde AS sindicato_localizacao_uf,
                    cut.id_unidade AS estabelecimento_id,
                    coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) AS empresa_id,
                    coalesce((case
	    		                when (ct.tipo_usuario_destino = 1)
	    			                then ct.usuario_destino_id
	    		                else NULL end),
	    	                (case
		    	                when (ct.tipo_usuario_destino = 2)
		    		                then cmt.cliente_grupo_id_grupo_economico
		    	                else NULL end), cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) AS grupo_economico_id
                from
                    (((((sind_emp se
                left join comentarios_tb ct on
                    (((ct.tipo = 2) and (se.id_sinde = ct.referencia_id))))
                left join usuario_adm adm on
                    ((adm.id_user = ct.usuario_inclusao_id)))
                left join cliente_unidades cut on
                    (((ct.tipo_usuario_destino = 3) and (ct.usuario_destino_id = cut.id_unidade))))
                left join cliente_matriz cmt on
                    (((ct.tipo_usuario_destino = 2) and (ct.usuario_destino_id = cmt.id_empresa))))
                left join cliente_grupo cgt on
                    (((ct.tipo_usuario_destino = 1) and (ct.usuario_destino_id = cgt.id_grupo_economico)))
                inner join etiqueta_tb et on et.id = ct.etiqueta_id)
                union all
                select
                    sp.id_sindp AS sindicato_id,
                    sp.sigla_sp AS sindicato_sigla,
                    sp.razaosocial_sp AS sindicato_razao_social,
                    'patronal' AS tipo,
                    CASE ct.tipo
                        WHEN 1 THEN 'Cláusula'
                        WHEN 2 THEN 'Sindicato Laboral'
                        WHEN 3 THEN 'Sindicato Patronal'
                        WHEN 4 THEN 'Filial'
                        ELSE ''
                    END AS notificacao_tipo_comentario,
                    ct.valor AS notificacao_comentario,
                    ct.id AS notificacao_id,
                    et.nome AS notificacao_etiqueta,
                    ct.data_inclusao AS notificacao_data_registro,
                    CASE ct.tipo_usuario_destino
                        WHEN 1 THEN 'Grupo'
                        WHEN 2 THEN 'Matriz'
                        WHEN 3 THEN 'Unidade'
                        ELSE ''
                    END AS notificacao_tipo_destino,
                    ct.usuario_destino_id AS notificacao_tipo_destino_id,
                    ct.visivel,
                    adm.id_user AS usuario_publicacao_id,
                    adm.nome_usuario AS usuario_publicacao,
                    adm.id_grupoecon AS usuario_publicacao_grupo_economico_id,
                    adm.nivel AS usuario_publicacao_tipo,
                    sp.uf_sp AS sindicato_localizacao_uf,
                    cut.id_unidade AS estabelecimento_id,
                    coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) AS empresa_id,
                    coalesce((case
	    		                when (ct.tipo_usuario_destino = 1)
	    			                then ct.usuario_destino_id
	    		                else NULL end),
	    	                (case
		    	                when (ct.tipo_usuario_destino = 2)
		    		                then cmt.cliente_grupo_id_grupo_economico
		    	                else NULL end), cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) AS grupo_economico_id
                from
                    (((((sind_patr sp
                left join comentarios_tb ct on
                    (((ct.tipo = 2) and (sp.id_sindp = ct.referencia_id))))
                left join usuario_adm adm on
                    ((adm.id_user = ct.usuario_inclusao_id)))
                left join cliente_unidades cut on
                    (((ct.tipo_usuario_destino = 3) and (ct.usuario_destino_id = cut.id_unidade))))
                left join cliente_matriz cmt on
                    (((ct.tipo_usuario_destino = 2) and (ct.usuario_destino_id = cmt.id_empresa))))
                left join cliente_grupo cgt on
                    (((ct.tipo_usuario_destino = 1) and (ct.usuario_destino_id = cgt.id_grupo_economico)))
                inner join etiqueta_tb et on et.id = ct.etiqueta_id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "etiqueta_id",
                table: "comentarios_tb",
                newName: "etiqueta");

            migrationBuilder.RenameIndex(
                name: "IX_comentarios_tb_etiqueta_id",
                table: "comentarios_tb",
                newName: "IX_comentarios_tb_etiqueta");

            migrationBuilder.Sql(@"DROP view if exists comentarios_vw;");

            migrationBuilder.Sql(@"DROP view if exists comentario_sindicato_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                ALGORITHM = UNDEFINED VIEW comentario_sindicato_vw AS
                select
                    se.id_sinde AS sindicato_id,
                    se.sigla_sinde AS sindicato_sigla,
                    se.razaosocial_sinde AS sindicato_razao_social,
                    'laboral' AS tipo,
                    nc.tipo_comentario AS notificacao_tipo_comentario,
                    nc.comentario AS notificacao_comentario,
                    nc.id_notecliente AS notificacao_id,
                    nc.etiqueta AS notificacao_etiqueta,
                    nc.data_registro AS notificacao_data_registro,
                    nc.tipo_usuario_destino AS notificacao_tipo_destino,
                    nc.id_tipo_usuario_destino AS notificacao_tipo_destino_id,
                    adm.id_user AS usuario_publicacao_id,
                    adm.nome_usuario AS usuario_publicacao,
                    adm.id_grupoecon AS usuario_publicacao_grupo_economico_id,
                    adm.nivel AS usuario_publicacao_tipo,
                    se.uf_sinde AS sindicato_localizacao_uf,
                    cut.id_unidade AS estabelecimento_id,
                    coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) AS empresa_id,
                    coalesce((case when (nc.tipo_usuario_destino = 'grupo') then nc.id_tipo_usuario_destino else NULL end),(case when (nc.tipo_usuario_destino = 'matriz') then cmt.cliente_grupo_id_grupo_economico else NULL end), cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) AS grupo_economico_id
                from
                    (((((sind_emp se
                left join note_cliente nc on
                    (((nc.tipo_comentario = 'laboral')
                        and (se.id_sinde = nc.id_tipo_comentario))))
                left join usuario_adm adm on
                    ((adm.id_user = nc.usuario_adm_id_user)))
                left join cliente_unidades cut on
                    (((nc.tipo_usuario_destino = 'unidade')
                        and (nc.id_tipo_usuario_destino = cut.id_unidade))))
                left join cliente_matriz cmt on
                    (((nc.tipo_usuario_destino = 'matriz')
                        and (nc.id_tipo_usuario_destino = cmt.id_empresa))))
                left join cliente_grupo cgt on
                    (((nc.tipo_usuario_destino = 'grupo')
                        and (nc.id_tipo_usuario_destino = cgt.id_grupo_economico))))
                union all
                select
                    sp.id_sindp AS sindicato_id,
                    sp.sigla_sp AS sindicato_sigla,
                    sp.razaosocial_sp AS sindicato_razao_social,
                    'patronal' AS tipo,
                    nc.tipo_comentario AS notificacao_tipo_comentario,
                    nc.comentario AS notificacao_comentario,
                    nc.id_notecliente AS notificacao_id,
                    nc.etiqueta AS notificacao_etiqueta,
                    nc.data_registro AS notificacao_data_registro,
                    nc.tipo_usuario_destino AS notificacao_tipo_destino,
                    nc.id_tipo_usuario_destino AS notificacao_tipo_destino_id,
                    adm.id_user AS usuario_publicacao_id,
                    adm.nome_usuario AS usuario_publicacao,
                    adm.id_grupoecon AS usuario_publicacao_grupo_economico_id,
                    adm.nivel AS usuario_publicacao_tipo,
                    sp.uf_sp AS sindicato_localizacao_uf,
                    cut.id_unidade AS estabelecimento_id,
                    coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) AS empresa_id,
                    coalesce((case when (nc.tipo_usuario_destino = 'grupo') then nc.id_tipo_usuario_destino else NULL end),(case when (nc.tipo_usuario_destino = 'matriz') then cmt.cliente_grupo_id_grupo_economico else NULL end), cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) AS grupo_economico_id
                from
                    (((((sind_patr sp
                left join note_cliente nc on
                    (((nc.tipo_comentario = 'patronal')
                        and (sp.id_sindp = nc.id_tipo_comentario))))
                left join usuario_adm adm on
                    ((adm.id_user = nc.usuario_adm_id_user)))
                left join cliente_unidades cut on
                    (((nc.tipo_usuario_destino = 'unidade')
                        and (nc.id_tipo_usuario_destino = cut.id_unidade))))
                left join cliente_matriz cmt on
                    (((nc.tipo_usuario_destino = 'matriz')
                        and (nc.id_tipo_usuario_destino = cmt.id_empresa))))
                left join cliente_grupo cgt on
                    (((nc.tipo_usuario_destino = 'grupo')
                        and (nc.id_tipo_usuario_destino = cgt.id_grupo_economico))));");
        }
    }
}
