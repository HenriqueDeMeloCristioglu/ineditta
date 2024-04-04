using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view documento_sindical_vw as
                                select dst.id_doc, tdt.nome_doc, dst.data_upload, dst.validade_inicial, dst.validade_final,
                                 tdt.sigla_doc sigla_doc,
                                 JSON_EXTRACT(dst.sind_laboral, '$[*].sigla') as sindicatos_laborais_siglas,
                                 JSON_EXTRACT(dst.sind_patronal , '$[*].sigla') as sindicatos_patronais_siglas, 
                                 JSON_EXTRACT(dst.sind_laboral, '$[*].municipio') sindicatos_laborais_municipios,
                                 dst.modulo,
                                 dst.sind_laboral,
                                 dst.sind_patronal,
                                 dst.cliente_estabelecimento,
                                 dst.referencia estrutura_clausulas_ids,
                                 dst.caminho_arquivo,
                                 dst.database_doc,
                                 dst.abrangencia,
                                 dst.cnae_doc,
                                 dst.descricao_documento,
                                 tdt.tipo_doc tipo_documento,
                                 dst.anuencia,
                                 dst.doc_restrito,
                                 dst.data_aprovacao,
                                 coalesce(cgt.quantidade_nao_aprovados,0) clausula_quantidade_nao_aprovadas,
                                 cgt.data_ultima_aprovacao clausula_data_ultima_aprovacao 
                                from doc_sind dst
                                left join tipo_doc tdt on dst.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                                left join lateral (select sum(case when cgt.aprovado = 'nao' then 1 else 0 end) quantidade_nao_aprovados, max(cgt.data_aprovacao) data_ultima_aprovacao
				                                   FROM clausula_geral as cgt 
				                                   where cgt.doc_sind_id_documento = dst.id_doc
				                                   group by cgt.doc_sind_id_documento) cgt on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documento_sindical_clausulas_vw;");
        }
    }
}
