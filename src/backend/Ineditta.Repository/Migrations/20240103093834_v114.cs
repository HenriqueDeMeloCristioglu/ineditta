using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v114 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_relatorio_vw;");

            migrationBuilder.Sql(@"create or replace view acompanhamento_cct_relatorio_vw as
                            select cct.idacompanhanto_cct id,
                                sempt.id_sinde AS id_sindicato_laboral,
	                            sempt.sigla_sinde AS sigla_sindicato_laboral,
                                sempt.cnpj_sinde AS cnpj_sindicato_laboral,
                                sempt.codigo_sinde AS codigo_sindicato_laboral,
                                sempt.uf_sinde AS uf_sindicato_laboral,
                                spt.id_sindp AS id_sindicato_patronal,
                                spt.sigla_sp AS sigla_sindicato_patronal,
                                spt.cnpj_sp AS cnpj_sindicato_patronal,
                                spt.codigo_sp AS codigo_sindicato_patronal,
                                spt.uf_sp AS uf_sindicato_patronal,
                                tdt.nome_doc AS nome_documento,
                                cct.data_base AS data_base,
                                cct.fase AS fase,
                                cct.observacoes_gerais AS observacoes_gerais,
                                date_format(cct.ultima_atualizacao, '%d/%m/%Y') AS ultima_atualizacao,
                            ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                            ir.indicador AS indicador,
                            ir.dado_real AS dado_real,
                            cct.ids_cnaes AS ids_cnaes,
                            date_format((ir.periodo_data + interval 1 month), '%b/%Y') AS ir_periodo,
                                cnaet.atividades_economicas,
                                estabelecimentos_grupo.estabelecimentos as estabelecimentos_grupo_economico,
                                estabelecimentos_matriz.estabelecimentos as estabelecimentos_matriz,
                                esindemp.estabelecimentos as estabelecimentos_laborais_principais,
                                esindpt.estabelecimentos as estabelecimentos_patronais_principais,
                                esindempadct.estabelecimentos as estabelecimentos_laborais_adicionais,
                                esindptadct.estabelecimentos as estabelecimentos_patronais_adicionais,
                                sind_ads_laborais.sindicatos as sindicatos_laborais_adicionais,
                                sind_ads_patronais.sindicatos as sindicatos_patronais_adicionais
                            from acompanhanto_cct cct
                            left join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde 
                            left join sind_patr spt on cct.sind_patr_id_sindp = spt.id_sindp 
                            left join tipo_doc tdt on cct.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                            left join indecon_real ir 
                                on ir.periodo_data is not null
                                and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                            and ir.indicador = 'INPC'
                            left join lateral (select group_concat(cnaet.descricao_subclasse separator ', ') as atividades_economicas
                                           from classe_cnae cnaet
                                           where json_contains(cct.ids_cnaes, concat('[""', cast(cnaet.id_cnae as char), '""]'))) cnaet on true
                            left join lateral (
			                             select 
			 	                            JSON_ARRAYAGG(
	 	                                      JSON_OBJECT('nome', cut.nome_unidade, 'cnpj', cut.cnpj_unidade, 'codigoSindicatoCliente', cut.cod_sindcliente)
		                                   ) AS estabelecimentos
			                             from cliente_unidades cut
                                         inner join localizacao lt on cut.localizacao_id_localizacao = lt.id_localizacao
                                         inner join base_territorialsindemp btset on lt.id_localizacao = btset.localizacao_id_localizacao1 AND JSON_CONTAINS(cut.cnae_unidade, CONCAT('{""id"":', btset.classe_cnae_idclasse_cnae,'}'))
                                         and sempt.id_sinde = btset.sind_empregados_id_sinde1) as esindemp on true
                            left join lateral (
			                             select 
			 	                            JSON_ARRAYAGG(
	 	                                      JSON_OBJECT('nome', cut.nome_unidade, 'cnpj', cut.cnpj_unidade, 'codigoSindicatoCliente', cut.cod_sindcliente)
		                                   ) AS estabelecimentos
			                             from cliente_unidades cut
                                         inner join localizacao lt on cut.localizacao_id_localizacao = lt.id_localizacao
                                         inner join base_territorialsindpatro bt on lt.id_localizacao = bt.localizacao_id_localizacao1 AND JSON_CONTAINS(cut.cnae_unidade, CONCAT('{""id"":', bt.classe_cnae_idclasse_cnae,'}'))
                                         and spt.id_sindp = bt.sind_patronal_id_sindp) as esindpt on true
                            left join lateral (
			                             select 
			 	                            JSON_ARRAYAGG(
	 	                                      JSON_OBJECT('nome', cut.nome_unidade, 'cnpj', cut.cnpj_unidade, 'codigoSindicatoCliente', cut.cod_sindcliente)
		                                   ) AS estabelecimentos
			                             from cliente_unidades cut
                                         inner join localizacao lt on cut.localizacao_id_localizacao = lt.id_localizacao
                                         inner join base_territorialsindemp btset on lt.id_localizacao = btset.localizacao_id_localizacao1 AND JSON_CONTAINS(cut.cnae_unidade, CONCAT('{""id"":', btset.classe_cnae_idclasse_cnae,'}'))
                                         and JSON_CONTAINS(cct.ids_sindemp_adicionais, JSON_ARRAY(btset.sind_empregados_id_sinde1))) as esindempadct on true
                            left join lateral (
			                             select 
			 	                            JSON_ARRAYAGG(
	 	                                      JSON_OBJECT('nome', cut.nome_unidade, 'cnpj', cut.cnpj_unidade, 'codigoSindicatoCliente', cut.cod_sindcliente)
		                                   ) AS estabelecimentos
				                             from cliente_unidades cut
	                                         inner join localizacao lt on cut.localizacao_id_localizacao = lt.id_localizacao
	                                         inner join base_territorialsindpatro bt on lt.id_localizacao = bt.localizacao_id_localizacao1 AND JSON_CONTAINS(cut.cnae_unidade, CONCAT('{""id"":', bt.classe_cnae_idclasse_cnae,'}'))
	                                         and JSON_CONTAINS(cct.ids_sindpatr_adicionais, JSON_ARRAY(bt.sind_patronal_id_sindp))
                                         ) as esindptadct on true
                            left join lateral (
				                             select JSON_ARRAYAGG(JSON_OBJECT('sigla', se.sigla_sinde, 'cnpj', se.cnpj_sinde, 'uf', se.uf_sinde)) AS sindicatos
				                             from sind_emp se
	                                         WHERE JSON_CONTAINS(cct.ids_sindemp_adicionais, JSON_ARRAY(se.id_sinde))
                                         ) as sind_ads_laborais on true
                            left join lateral (
				                             select JSON_ARRAYAGG(JSON_OBJECT('sigla', sp.sigla_sp, 'cnpj', sp.cnpj_sp, 'uf', sp.uf_sp)) AS sindicatos
				                             from sind_patr sp
	                                         WHERE JSON_CONTAINS(cct.ids_sindpatr_adicionais, JSON_ARRAY(sp.id_sindp))
                                         ) as sind_ads_patronais on true
                            left join lateral (
				                             select JSON_ARRAYAGG(
				 		                            JSON_OBJECT('nome', cut.nome_unidade, 'cnpj', cut.cnpj_unidade, 'codigoSindicatoCliente', cut.cod_sindcliente)
				 	                            ) AS estabelecimentos
				 	                            from cliente_unidades cut
	                                         WHERE JSON_CONTAINS(cct.grupos_economicos_ids, JSON_ARRAY(cut.cliente_grupo_id_grupo_economico))
                                         ) as estabelecimentos_grupo on true
                            left join lateral (
				                             select JSON_ARRAYAGG(
				 	                            JSON_OBJECT('nome', cut.nome_unidade, 'cnpj', cut.cnpj_unidade, 'codigoSindicatoCliente', cut.cod_sindcliente)
				                             ) AS estabelecimentos
				                             from cliente_unidades cut
	                                         WHERE JSON_CONTAINS(cct.empresas_ids, JSON_ARRAY(cut.cliente_matriz_id_empresa))
                                         ) as estabelecimentos_matriz on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_relatorio_vw;");
        }
    }
}
