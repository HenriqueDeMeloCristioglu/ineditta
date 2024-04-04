using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v91 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_item_vw;");
            
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW comparativo_mapa_sindical_item_vw AS
                                    select
                                        cgt.id_clau AS clausula_id,
                                        cgect.id_clausulageral_estrutura_clausula AS clausula_geral_estrutura_id,
                                        cgt.doc_sind_id_documento AS documento_sindical_id,
                                        gct.idgrupo_clausula AS grupo_clausula_id,
                                        gct.nome_grupo AS grupo_clausula_nome,
                                        tiat.cdtipoinformacaoadicional AS informacao_adicional_id,
                                        tiat.nmtipoinformacaoadicional AS informacao_adicional_nome,
                                        cgect.data AS valor_data,
                                        cgect.numerico AS valor_numerico,
                                        cgect.texto AS valor_texto,
                                        cgect.percentual AS valor_percentual,
                                        cgect.descricao AS valor_descricao,
                                        cgect.hora AS valor_hora,
                                        cgect.combo AS valor_combo,
                                        cgect.sequencia AS sequencia,
                                        cgect.grupo_dados AS grupo_dados,
                                        dsindt.titulo_documento AS documento_titulo,
                                        dsindt.data_aprovacao AS documento_data_aprovacao,
                                        dsindt.sind_patronal AS documento_sindicatos_patronais,
                                        dsindt.sind_laboral AS documento_sindicatos_laborais,
                                        dsindt.cnae_doc AS documento_atividades_economicas,
                                        dsindt.uf AS documento_uf,
                                        dsindt.validade_inicial AS documento_validade_inicial,
                                        dsindt.validade_final AS documento_validade_final,
                                        dsindt.database_doc AS documento_database,
                                        ect.tipo_clausula AS estrutura_clausula_tipo,
                                        tiat.idtipodado AS informacao_adicional_tipo_dado,
                                        cgect.id_info_tipo_grupo AS informacao_adicional_grupo_id,
                                        coalesce(cgect.nome_informacao, 0) AS clausula_grupo_informacao_adicional_nome,
                                        (case
                                            when (cgt.aprovador = 'sim') then true
                                            else false
                                        end) AS clausula_aprovada,
                                        cgt.data_aprovacao AS clausula_data_aprovacao,
                                        cgt.tex_clau AS clausula_texto,
                                        (case
                                            when (cgt.liberado = 'S') then true
                                            else false
                                        end) AS clausula_liberada,
                                        td.nome_doc AS tipo_documento_nome,
                                        dsindt.abrangencia AS documento_abrangencia,
                                        coalesce(iagt.exibe_comparativo_mapa_sindical, 0) AS exibe_comparativo_mapa_sindical
                                    from
                                        clausula_geral cgt
                                    join clausula_geral_estrutura_clausula cgect on
                                        cgt.id_clau = cgect.clausula_geral_id_clau
                                    join estrutura_clausula ect on
                                        cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
                                    join grupo_clausula gct on
                                        ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                    join ad_tipoinformacaoadicional tiat on
                                        cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = tiat.cdtipoinformacaoadicional
                                    join doc_sind dsindt on
                                        cgt.doc_sind_id_documento = dsindt.id_doc
                                    join tipo_doc td on
                                        dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                                    left join lateral(select iagt.* from estrutura_clausulas_ad_tipoinformacaoadicional ecat 
				                                      inner join informacao_adicional_grupo iagt on ecat.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.ad_tipoinformacaoadicional_id
			                                          where cgect.nome_informacao = ecat.estrutura_clausula_id_estruturaclausula
		                                              and cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.informacaoadicional_no_grupo
			                                     limit 1) iagt on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_item_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW comparativo_mapa_sindical_item_vw AS
                                select
                                    cgt.id_clau AS clausula_id,
                                    cgect.id_clausulageral_estrutura_clausula AS clausula_geral_estrutura_id,
                                    cgt.doc_sind_id_documento AS documento_sindical_id,
                                    gct.idgrupo_clausula AS grupo_clausula_id,
                                    gct.nome_grupo AS grupo_clausula_nome,
                                    tiat.cdtipoinformacaoadicional AS informacao_adicional_id,
                                    tiat.nmtipoinformacaoadicional AS informacao_adicional_nome,
                                    cgect.data AS valor_data,
                                    cgect.numerico AS valor_numerico,
                                    cgect.texto AS valor_texto,
                                    cgect.percentual AS valor_percentual,
                                    cgect.descricao AS valor_descricao,
                                    cgect.hora AS valor_hora,
                                    cgect.combo AS valor_combo,
                                    cgect.sequencia AS sequencia,
                                    cgect.grupo_dados AS grupo_dados,
                                    dsindt.titulo_documento AS documento_titulo,
                                    dsindt.data_aprovacao AS documento_data_aprovacao,
                                    dsindt.sind_patronal AS documento_sindicatos_patronais,
                                    dsindt.sind_laboral AS documento_sindicatos_laborais,
                                    dsindt.cnae_doc AS documento_atividades_economicas,
                                    dsindt.uf AS documento_uf,
                                    dsindt.validade_inicial AS documento_validade_inicial,
                                    dsindt.validade_final AS documento_validade_final,
                                    dsindt.database_doc AS documento_database,
                                    ect.tipo_clausula AS estrutura_clausula_tipo,
                                    tiat.idtipodado AS informacao_adicional_tipo_dado,
                                    cgect.id_info_tipo_grupo AS informacao_adicional_grupo_id,
                                    coalesce(cgect.nome_informacao, 0) AS clausula_grupo_informacao_adicional_nome,
                                    (case
                                        when (cgt.aprovador = 'sim') then true
                                        else false
                                    end) AS clausula_aprovada,
                                    cgt.data_aprovacao AS clausula_data_aprovacao,
                                    cgt.tex_clau AS clausula_texto,
                                    (case
                                        when (cgt.liberado = 'S') then true
                                        else false
                                    end) AS clausula_liberada,
                                    td.nome_doc AS tipo_documento_nome,
                                    dsindt.abrangencia AS documento_abrangencia,
                                    coalesce(iagt.exibe_comparativo_mapa_sindical, 0) AS exibe_comparativo_mapa_sindical
                                from
                                    BancoIneditta_PROD_T1.clausula_geral cgt
                                join BancoIneditta_PROD_T1.clausula_geral_estrutura_clausula cgect on
                                    cgt.id_clau = cgect.clausula_geral_id_clau
                                join BancoIneditta_PROD_T1.estrutura_clausula ect on
                                    cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
                                join BancoIneditta_PROD_T1.grupo_clausula gct on
                                    ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                join BancoIneditta_PROD_T1.ad_tipoinformacaoadicional tiat on
                                    cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = tiat.cdtipoinformacaoadicional
                                join BancoIneditta_PROD_T1.doc_sind dsindt on
                                    cgt.doc_sind_id_documento = dsindt.id_doc
                                join BancoIneditta_PROD_T1.tipo_doc td on
                                    dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                                left join BancoIneditta_PROD_T1.informacao_adicional_grupo iagt on
                                    cgect.id_info_tipo_grupo = iagt.ad_tipoinformacaoadicional_id
                                        and cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.informacaoadicional_no_grupo;");
        }
    }
}
