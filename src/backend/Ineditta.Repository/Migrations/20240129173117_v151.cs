using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v151 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS calendario_sindical_assembleia_reuniao_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW calendario_sindical_assembleia_reuniao_vw AS
                                    select
                                        cst.data_referencia AS data_referencia,
                                        (case
                                            when (cst.origem = 1) then 'Ineditta'
                                            else 'Cliente'
                                        end) AS origem,
                                        (case
                                            when (cst.tipo_evento = 7) then 'Assembleia patronal com as empresas'
                                            else 'Reunião entre entidades sindicais'
                                        end) AS tipo_evento,
                                        ac.cnaes_ids AS atividades_economicas_ids,
                                        ac.data_base AS data_base,
                                        td.idtipo_doc AS tipo_doc_id,
                                        td.nome_doc AS nome_documento,
                                        fc.fase_negociacao AS fase_documento,
                                        cst.chave_referencia AS chave_referencia,
                                        ac.grupos_economicos_ids AS grupos_economicos_ids,
                                        ac.empresas_ids AS matrizes_ids,
                                        cct.descricoes_subclasses AS descricoes_subclasses,
                                        cst.tipo_evento AS tipo_evento_id,
                                        sindicatos_laborais.sindicatos sindicatos_laborais,
                                        sindicatos_patronais.sindicatos sindicatos_patronais
                                    from calendario_sindical_tb cst
                                    join acompanhamento_cct_tb ac on ac.id = cst.chave_referencia
                                    join fase_cct fc on ac.fase_id = fc.id_fase
                                    left join lateral (
	                                    select json_arrayagg(
		                                    json_object(
			                                    'id',
			                                    se.id_sinde,
			                                    'sigla',
			                                    se.sigla_sinde
		                                    )
	                                    ) as sindicatos from acompanhamento_cct_sindicato_laboral_tb acslt
	                                    inner join sind_emp se on acslt.sindicato_id = se.id_sinde
	                                    WHERE acslt.acompanhamento_cct_id = ac.id
                                    ) sindicatos_laborais on true
                                    left join lateral (
	                                    select json_arrayagg(
		                                    json_object(
			                                    'id',
			                                    sp.id_sindp,
			                                    'sigla',
			                                    sp.sigla_sp
		                                    )
	                                    ) as sindicatos from acompanhamento_cct_sindicato_patronal_tb acspt
	                                    inner join sind_patr sp on acspt.sindicato_id = sp.id_sindp 
	                                    WHERE acspt.acompanhamento_cct_id = ac.id
                                    ) sindicatos_patronais on true
                                    left join tipo_doc td on td.idtipo_doc = ac.tipo_documento_id
                                    left join lateral (
                                        select group_concat(cct.descricao_subclasse separator ';') AS descricoes_subclasses
                                        from classe_cnae cct where json_contains(ac.cnaes_ids, json_array(cast(cct.id_cnae as char charset utf8mb4)))
                                    ) cct on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS calendario_sindical_assembleia_reuniao_vw;");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW calendario_sindical_assembleia_reuniao_vw AS
                SELECT
                    cst.data_referencia data_referencia,
                    CASE WHEN cst.origem = 1 THEN 'Ineditta' ELSE 'Cliente' END AS origem,
                    CASE WHEN cst.tipo_evento = 7 THEN 'Assembleia patronal com as empresas' ELSE 'Reunião entre entidades sindicais' END AS tipo_evento,
                    ac.sind_emp_id_sinde sindicato_laboral_id,
                    ac.sind_patr_id_sindp sindicato_patronal_id,
                    se.sigla_sinde sindicato_laboral_sigla,
                    sp.sigla_sp sindicato_patronal_sigla,
                    ac.ids_cnaes atividades_economicas_ids,
                    ac.data_base data_base,
                    td.idtipo_doc tipo_doc_id,
                    td.nome_doc nome_documento,
                    ac.fase fase_documento,
                    cst.chave_referencia chave_referencia,
                    ac.grupos_economicos_ids grupos_economicos_ids,
                    ac.empresas_ids matrizes_ids,
                    cct.descricoes_subclasses descricoes_subclasses,
                    cst.tipo_evento tipo_evento_id
                FROM calendario_sindical_tb cst
                JOIN acompanhanto_cct ac ON ac.idacompanhanto_cct = cst.chave_referencia
                LEFT JOIN sind_emp se ON se.id_sinde = ac.sind_emp_id_sinde
                LEFT JOIN sind_patr sp ON sp.id_sindp = ac.sind_patr_id_sindp
                LEFT JOIN tipo_doc td ON td.idtipo_doc = ac.tipo_doc_idtipo_doc 
                LEFT JOIN LATERAL (SELECT GROUP_concat(cct.descricao_subclasse separator ';') descricoes_subclasses FROM classe_cnae cct
                                   where JSON_CONTAINS(ac.ids_cnaes, JSON_ARRAY( cast(cct.id_cnae as char)))) cct on true
            ");
        }
    }
}
