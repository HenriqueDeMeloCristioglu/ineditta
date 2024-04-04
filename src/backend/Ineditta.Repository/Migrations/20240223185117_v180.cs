using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v180 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_documento_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_documento_vw AS
                SELECT
	                ds.validade_final `data`,
	                'Vencimento de Documentos' tipo_evento,
	                'Ineditta' origem,
	                ds.validade_inicial validade_inicial,
	                ds.validade_final validade_final,
	                ds.tipo_doc_idtipo_doc tipo_doc_id,
	                td.nome_doc tipo_doc_nome,
	                ds.cnae_doc atividades_economicas_json,
	                ds.cliente_estabelecimento cliente_estabelecimento,
	                ds.sind_laboral,
	                ds.sind_patronal,
	                ds.referencia,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas
                FROM doc_sind ds
                LEFT JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE ds.modulo = 'SISAP'
                      AND ds.validade_final <> '0000-00-00'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_documento_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_documento_vw AS
                SELECT
	                ds.validade_final `data`,
	                'Vencimento de Documentos' tipo_evento,
	                'Ineditta' origem,
	                ds.validade_inicial validade_inicial,
	                ds.validade_final validade_final,
	                ds.tipo_doc_idtipo_doc tipo_doc_id,
	                td.nome_doc tipo_doc_nome,
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
                FROM doc_sind ds
                LEFT JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE ds.modulo = 'SISAP'
                      AND ds.validade_final <> '0000-00-00'
            ");
        }
    }
}
