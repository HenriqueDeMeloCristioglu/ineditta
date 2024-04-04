using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v156 : Migration
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
	                                    sindicatos_patronais.denominacoes denominacoes_patronais,
	                                    dsindt.sind_laboral documento_sindicatos_laborais,
	                                    sindicatos_laborais.denominacoes denominacoes_laborais,
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
	                                    cgect.estrutura_clausula_id_estruturaclausula estrutura_clausula_id,
	                                    dsindt.cliente_estabelecimento estabelecimentos_json,
	                                    dsindt.sind_laboral AS sind_laboral,
	                                    dsindt.sind_patronal AS sind_patronal
	                                from clausula_geral cgt
	                                inner join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau 
	                                inner join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula 
	                                inner join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula 
	                                inner join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional  = tiat.cdtipoinformacaoadicional 
	                                inner join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
	                                inner join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
	                                inner join lateral (
	                                    SELECT 
	                                        GROUP_CONCAT(sp.denominacao_sp separator '; ') AS denominacoes 
	                                    FROM sind_patr sp
	                                    INNER JOIN documento_sindicato_patronal_tb dspt ON dspt.documento_id = cgt.doc_sind_id_documento
	                                    WHERE sp.id_sindp = dspt.sindicato_patronal_id
	                                ) sindicatos_patronais
	                                inner join lateral (
	                                    SELECT 
	                                        GROUP_CONCAT(sinde.denominacao_sinde separator '; ') AS denominacoes 
	                                    FROM sind_emp sinde
	                                    INNER JOIN documento_sindicato_laboral_tb dslt ON dslt.documento_id = cgt.doc_sind_id_documento
	                                    WHERE sinde.id_sinde = dslt.sindicato_laboral_id
	                                ) sindicatos_laborais;
	                            ");
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
	                                    sindicatos_patronais.denominacoes denominacoes_patronais,
	                                    dsindt.sind_laboral documento_sindicatos_laborais,
	                                    sindicatos_laborais.denominacoes denominacoes_laborais,
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
	                                    cgect.estrutura_clausula_id_estruturaclausula estrutura_clausula_id,
	                                    unidades.codigos codigos_unidades,
	                                    unidades.cnpjs cnpjs_unidades,
	                                    unidades.codigos_sindicato_cliente codigos_sindicato_cliente_unidades,
	                                    dsindt.cliente_estabelecimento estabelecimentos_json,
	                                    dsindt.sind_laboral AS sind_laboral,
                                        dsindt.sind_patronal AS sind_patronal
                                    from clausula_geral cgt
                                    inner join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau 
                                    inner join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula 
                                    inner join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula 
                                    inner join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional  = tiat.cdtipoinformacaoadicional 
                                    inner join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                                    inner join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                                    inner join lateral (
	                                    SELECT 
		                                    GROUP_CONCAT(DISTINCT cut.codigo_unidade separator ', ') AS codigos, 
		                                    GROUP_CONCAT(DISTINCT cut.cnpj_unidade separator ', ') AS cnpjs,
		                                    GROUP_CONCAT(DISTINCT cut.cod_sindcliente separator ', ') AS codigos_sindicato_cliente
	                                    FROM cliente_unidades cut
	                                    INNER JOIN doc_sind dsindt2 on cgt.doc_sind_id_documento = dsindt2.id_doc,
	                                    json_table(dsindt2.cliente_estabelecimento, '$[*].u' columns (value int path '$')) as cus
	                                    WHERE cus.value = cut.id_unidade
                                    ) unidades
                                    inner join lateral (
	                                    SELECT 
		                                    GROUP_CONCAT(DISTINCT sp.denominacao_sp separator '; ') AS denominacoes 
	                                    FROM sind_patr sp
	                                    INNER JOIN doc_sind dsindt2 on cgt.doc_sind_id_documento = dsindt2.id_doc,
	                                    json_table(dsindt2.sind_patronal , '$[*].id' columns (value int path '$')) as sps
	                                    WHERE sps.value = sp.id_sindp
                                    ) sindicatos_patronais
                                    inner join lateral (
	                                    SELECT 
		                                    GROUP_CONCAT(DISTINCT sinde.denominacao_sinde separator '; ') AS denominacoes 
	                                    FROM sind_emp sinde
	                                    INNER JOIN doc_sind dsindt2 on cgt.doc_sind_id_documento = dsindt2.id_doc,
	                                    json_table(dsindt2.sind_laboral, '$[*].id' columns (value int path '$')) as ses
	                                    WHERE ses.value = sinde.id_sinde
                                    ) sindicatos_laborais;
	                            ");
        }
    }
}
