using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v208 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_trintidio_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_trintidio_vw AS
                SELECT
                    cs.data_referencia AS `data`,
                    'Trintídio' grupo,
                    CASE WHEN cs.origem = 1 THEN 'Ineditta' ELSE 'Cliente' END AS origem,
                    DATE_SUB(ds.validade_inicial, INTERVAL 60 DAY) validade_inicial,
	                DATE_SUB(ds.validade_final, INTERVAL 29 DAY) validade_final,
	                ds.tipo_doc_idtipo_doc tipo_doc,
	                td.nome_doc,
	                ds.validade_inicial database_doc,
	                ds.cnae_doc atividades_economicas_json,
	                ds.cliente_estabelecimento cliente_estabelecimento,
	                ds.sind_laboral,
	                ds.sind_patronal,
	                ds.referencia,
                    cs.chave_referencia,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais_ids,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_laborais,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais_ids,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_patronais
                FROM
                    calendario_sindical_tb cs
	            JOIN doc_sind ds ON ds.id_doc = cs.chave_referencia
                JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE
                    cs.tipo_evento = 4
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_trintidio_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_trintidio_vw AS
                SELECT 
	                DATE_SUB(ds.validade_inicial, INTERVAL 60 DAY) `data`,
	                'Trintídio' grupo,
	                'Ineditta' origem,
	                DATE_SUB(ds.validade_inicial, INTERVAL 60 DAY) validade_inicial,
	                DATE_SUB(ds.validade_final, INTERVAL 29 DAY) validade_final,
	                ds.tipo_doc_idtipo_doc tipo_doc,
	                td.nome_doc,
	                ds.validade_inicial database_doc,
	                ds.cnae_doc atividades_economicas_json,
	                ds.cliente_estabelecimento cliente_estabelecimento,
	                ds.sind_laboral,
	                ds.sind_patronal,
	                ds.referencia,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais_ids,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_laborais,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais_ids,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_patronais
                FROM
	                doc_sind ds
                JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE
	                ds.tipo_doc_idtipo_doc in (5,6)
	                OR (ds.tipo_doc_idtipo_doc in (8,9) and JSON_CONTAINS(ds.referencia, JSON_ARRAY('84')))
	                and ds.modulo = 'SISAP'
            ");
        }
    }
}
