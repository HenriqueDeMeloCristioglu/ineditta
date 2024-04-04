using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP view if exists comentario_sindicato_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                                ALGORITHM = UNDEFINED VIEW `comentario_sindicato_vw` AS
                                select
                                    `se`.`id_sinde` AS `sindicato_id`,
                                    `se`.`sigla_sinde` AS `sindicato_sigla`,
                                    `se`.`razaosocial_sinde` AS `sindicato_razao_social`,
                                    'laboral' AS `tipo`,
                                    (case
                                        `ct`.`tipo` when 1 then 'Cláusula'
                                        when 2 then 'Sindicato Laboral'
                                        when 3 then 'Sindicato Patronal'
                                        when 4 then 'Filial'
                                        else ''
                                    end) AS `notificacao_tipo_comentario`,
                                    `ct`.`valor` AS `notificacao_comentario`,
                                    `ct`.`id` AS `notificacao_id`,
                                    `et`.`nome` AS `notificacao_etiqueta`,
                                    `ct`.`data_inclusao` AS `notificacao_data_registro`,
                                    (case
                                        `ct`.`tipo_usuario_destino` when 1 then 'Grupo'
                                        when 2 then 'Matriz'
                                        when 3 then 'Unidade'
                                        else ''
                                    end) AS `notificacao_tipo_destino`,
                                    `ct`.`usuario_destino_id` AS `notificacao_tipo_destino_id`,
                                    `ct`.`visivel` AS `visivel`,
                                    `adm`.`id_user` AS `usuario_publicacao_id`,
                                    `adm`.`nome_usuario` AS `usuario_publicacao`,
                                    `adm`.`id_grupoecon` AS `usuario_publicacao_grupo_economico_id`,
                                    `adm`.`nivel` AS `usuario_publicacao_tipo`,
                                    `se`.`uf_sinde` AS `sindicato_localizacao_uf`,
                                    `cut`.`id_unidade` AS `estabelecimento_id`,
                                    coalesce(`cmt`.`id_empresa`, `cut`.`cliente_matriz_id_empresa`) AS `empresa_id`,
                                    coalesce((case when (`ct`.`tipo_usuario_destino` = 1) then `ct`.`usuario_destino_id` else NULL end),(case when (`ct`.`tipo_usuario_destino` = 2) then `cmt`.`cliente_grupo_id_grupo_economico` else NULL end), `cgt`.`id_grupo_economico`, `cut`.`cliente_grupo_id_grupo_economico`) AS `grupo_economico_id`
                                from
                                    ((((((`sind_emp` `se`
                                left join `comentarios_tb` `ct` on
                                    (((`ct`.`tipo` = 2)
                                        and (`se`.`id_sinde` = `ct`.`referencia_id`))))
                                left join `usuario_adm` `adm` on
                                    ((`adm`.`id_user` = `ct`.`usuario_inclusao_id`)))
                                left join `cliente_unidades` `cut` on
                                    (((`ct`.`tipo_usuario_destino` = 3)
                                        and (`ct`.`usuario_destino_id` = `cut`.`id_unidade`))))
                                left join `cliente_matriz` `cmt` on
                                    (((`ct`.`tipo_usuario_destino` = 2)
                                        and (`ct`.`usuario_destino_id` = `cmt`.`id_empresa`))))
                                left join `cliente_grupo` `cgt` on
                                    (((`ct`.`tipo_usuario_destino` = 1)
                                        and (`ct`.`usuario_destino_id` = `cgt`.`id_grupo_economico`))))
                                left join `etiqueta_tb` `et` on
                                    ((`et`.`id` = `ct`.`etiqueta_id`)))
                                union all
                                select
                                    `sp`.`id_sindp` AS `sindicato_id`,
                                    `sp`.`sigla_sp` AS `sindicato_sigla`,
                                    `sp`.`razaosocial_sp` AS `sindicato_razao_social`,
                                    'patronal' AS `tipo`,
                                    (case
                                        `ct`.`tipo` when 1 then 'Cláusula'
                                        when 2 then 'Sindicato Laboral'
                                        when 3 then 'Sindicato Patronal'
                                        when 4 then 'Filial'
                                        else ''
                                    end) AS `notificacao_tipo_comentario`,
                                    `ct`.`valor` AS `notificacao_comentario`,
                                    `ct`.`id` AS `notificacao_id`,
                                    `et`.`nome` AS `notificacao_etiqueta`,
                                    `ct`.`data_inclusao` AS `notificacao_data_registro`,
                                    (case
                                        `ct`.`tipo_usuario_destino` when 1 then 'Grupo'
                                        when 2 then 'Matriz'
                                        when 3 then 'Unidade'
                                        else ''
                                    end) AS `notificacao_tipo_destino`,
                                    `ct`.`usuario_destino_id` AS `notificacao_tipo_destino_id`,
                                    `ct`.`visivel` AS `visivel`,
                                    `adm`.`id_user` AS `usuario_publicacao_id`,
                                    `adm`.`nome_usuario` AS `usuario_publicacao`,
                                    `adm`.`id_grupoecon` AS `usuario_publicacao_grupo_economico_id`,
                                    `adm`.`nivel` AS `usuario_publicacao_tipo`,
                                    `sp`.`uf_sp` AS `sindicato_localizacao_uf`,
                                    `cut`.`id_unidade` AS `estabelecimento_id`,
                                    coalesce(`cmt`.`id_empresa`, `cut`.`cliente_matriz_id_empresa`) AS `empresa_id`,
                                    coalesce((case when (`ct`.`tipo_usuario_destino` = 1) then `ct`.`usuario_destino_id` else NULL end),(case when (`ct`.`tipo_usuario_destino` = 2) then `cmt`.`cliente_grupo_id_grupo_economico` else NULL end), `cgt`.`id_grupo_economico`, `cut`.`cliente_grupo_id_grupo_economico`) AS `grupo_economico_id`
                                from
                                    ((((((`sind_patr` `sp`
                                left join `comentarios_tb` `ct` on
                                    (((`ct`.`tipo` = 3)
                                        and (`sp`.`id_sindp` = `ct`.`referencia_id`))))
                                left join `usuario_adm` `adm` on
                                    ((`adm`.`id_user` = `ct`.`usuario_inclusao_id`)))
                                left join `cliente_unidades` `cut` on
                                    (((`ct`.`tipo_usuario_destino` = 3)
                                        and (`ct`.`usuario_destino_id` = `cut`.`id_unidade`))))
                                left join `cliente_matriz` `cmt` on
                                    (((`ct`.`tipo_usuario_destino` = 2)
                                        and (`ct`.`usuario_destino_id` = `cmt`.`id_empresa`))))
                                left join `cliente_grupo` `cgt` on
                                    (((`ct`.`tipo_usuario_destino` = 1)
                                        and (`ct`.`usuario_destino_id` = `cgt`.`id_grupo_economico`))))
                                left join `etiqueta_tb` `et` on
                                    ((`et`.`id` = `ct`.`etiqueta_id`)));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
