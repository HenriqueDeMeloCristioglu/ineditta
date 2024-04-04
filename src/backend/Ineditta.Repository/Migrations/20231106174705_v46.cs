using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists clausula_geral_info_adicional_vw;");

            migrationBuilder.Sql(@"	create or replace view clausula_geral_info_adicional_vw as
	                            select cgt.id_clau clausula_id,
	                            cgect.id_clausulageral_estrutura_clausula clausula_geral_estrutura_id,
	                            cgt.doc_sind_id_documento documento_sindical_id,
	                            gct.idgrupo_clausula grupo_clausula_id,
	                            gct.nome_grupo grupo_clausula_nome,
	                            tiat.cdtipoinformacaoadicional informacao_adicional_id,
	                            tiat.nmtipoinformacaoadicional informacao_adicional_nome,
                                cgect.`data` valor_data,
                                cgect.numerico valor_numerico,
                                cgect.texto valor_texto,
                                cgect.percentual valor_percentual,
                                cgect.descricao valor_descricao,
                                cgect.hora valor_hora,
                                cgect.combo valor_combo,
                                cgect.sequencia sequencia,
                                cgect.grupo_dados grupo_dados,
                                dsindt.titulo_documento documento_titulo,
                                dsindt.data_aprovacao documento_data_aprovacao,
                                dsindt.sind_patronal documento_sindicatos_patronais,
                                dsindt.sind_laboral documento_sindicatos_laborais,
                                dsindt.cnae_doc documento_atividades_economicas,
                                dsindt.uf documento_uf,
                                dsindt.validade_inicial documento_validade_inicial,
                                dsindt.validade_final documento_validade_final,
                                dsindt.database_doc documento_database,
                                ect.tipo_clausula estrutura_clausula_tipo,
                                tiat.idtipodado informacao_adicional_tipo_dado,
                                case when cgt.aprovador = 'sim' then true else false end clausula_aprovada,
                                cgt.data_aprovacao clausula_data_aprovacao,
                                cgt.tex_clau clausula_texto,
                                case when cgt.liberado = 'S' then true else false end clausula_liberada,
                                td.nome_doc as tipo_documento_nome,
                                dsindt.abrangencia AS documento_abrangencia,
                                cgect.estrutura_clausula_id_estruturaclausula estrutura_clausula_id
	                            from clausula_geral cgt
	                            inner join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau 
	                            inner join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula 
	                            inner join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula 
	                            inner join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional  = tiat.cdtipoinformacaoadicional 
	                            inner join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                                inner join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc ;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists clausula_geral_info_adicional_vw;");

            migrationBuilder.Sql(@"	create or replace view clausula_geral_info_adicional_vw as
	                            select cgt.id_clau clausula_id,
	                            cgect.id_clausulageral_estrutura_clausula clausula_geral_estrutura_id,
	                            cgt.doc_sind_id_documento documento_sindical_id,
	                            gct.idgrupo_clausula grupo_clausula_id,
	                            gct.nome_grupo grupo_clausula_nome,
	                            tiat.cdtipoinformacaoadicional informacao_adicional_id,
	                            tiat.nmtipoinformacaoadicional informacao_adicional_nome,
                                cgect.`data` valor_data,
                                cgect.numerico valor_numerico,
                                cgect.texto valor_texto,
                                cgect.percentual valor_percentual,
                                cgect.descricao valor_descricao,
                                cgect.hora valor_hora,
                                cgect.combo valor_combo,
                                cgect.sequencia sequencia,
                                cgect.grupo_dados grupo_dados,
                                dsindt.titulo_documento documento_titulo,
                                dsindt.data_aprovacao documento_data_aprovacao,
                                dsindt.sind_patronal documento_sindicatos_patronais,
                                dsindt.sind_laboral documento_sindicatos_laborais,
                                dsindt.cnae_doc documento_atividades_economicas,
                                dsindt.uf documento_uf,
                                dsindt.validade_inicial documento_validade_inicial,
                                dsindt.validade_final documento_validade_final,
                                dsindt.database_doc documento_database,
                                ect.tipo_clausula estrutura_clausula_tipo,
                                tiat.idtipodado informacao_adicional_tipo_dado,
                                case when cgt.aprovador = 'sim' then true else false end clausula_aprovada,
                                cgt.data_aprovacao clausula_data_aprovacao,
                                cgt.tex_clau clausula_texto,
                                case when cgt.liberado = 'S' then true else false end clausula_liberada,
                                td.nome_doc as tipo_documento_nome,
                                dsindt.abrangencia AS documento_abrangencia
	                            from clausula_geral cgt
	                            inner join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau 
	                            inner join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula 
	                            inner join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula 
	                            inner join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional  = tiat.cdtipoinformacaoadicional 
	                            inner join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                                inner join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc ;");
        }
    }
}
