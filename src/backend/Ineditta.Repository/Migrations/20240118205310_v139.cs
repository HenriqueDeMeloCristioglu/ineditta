using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v139 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE if exists inserir_estabelecimentos_sindicatos_acompanhamento;");

            migrationBuilder.Sql(@"CREATE PROCEDURE inserir_estabelecimentos_sindicatos_acompanhamento()
                                    BEGIN
                                        truncate acompanhamento_cct_estabelecimento_tb;
                                        truncate acompanhamento_cct_sindicato_laboral_tb;
                                        truncate acompanhamento_cct_sindicato_patronal_tb;

   	                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                        select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id
                                        from acompanhanto_cct cct 
                                        inner join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde
                                        inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1 and json_contains(cct.ids_cnaes, json_array(cast(btset.classe_cnae_idclasse_cnae as char)))
                                        inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                                        inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao
                                        inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                                            where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                           where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                                           and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                           and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                           and cut.id_unidade = acet.estabelecimento_id)
                                             and exists(select 1 from sind_patr sptr 
                                                                 inner join base_territorialsindpatro btspt on sptr.id_sindp = btspt.sind_patronal_id_sindp
                                                                 inner join cnae_emp cet2 on cut.id_unidade = cet2.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet2.classe_cnae_idclasse_cnae
                                                                 where (cct.sind_patr_id_sindp = sptr.id_sindp 
                                                                 or json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]')))
                                                                 and cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1
                                             )
                                           and cut.cliente_grupo_id_grupo_economico is not null
                                           and cut.cliente_matriz_id_empresa is not null;
      
      
  	                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                        select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id
                                        from acompanhanto_cct cct 
                                        inner join sind_emp sempt on cct.sind_emp_id_sinde = json_contains(cct.ids_sindemp_adicionais, concat('[""', sempt.id_sinde, '""]'))
                                        inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1 and json_contains(cct.ids_cnaes, json_array(cast(btset.classe_cnae_idclasse_cnae as char)))
                                        inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                                        inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao
                                        inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                                            where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                           where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                                           and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                           and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                           and cut.id_unidade = acet.estabelecimento_id)
                                             and exists(select 1 from sind_patr sptr 
                                                                 inner join base_territorialsindpatro btspt on sptr.id_sindp = btspt.sind_patronal_id_sindp
                                                                 inner join cnae_emp cet2 on cut.id_unidade = cet2.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet2.classe_cnae_idclasse_cnae
                                                                 where (cct.sind_patr_id_sindp = sptr.id_sindp 
                                                                 or json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]')))
                                                                 and cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1
                                             )
                                           and cut.cliente_grupo_id_grupo_economico is not null
                                           and cut.cliente_matriz_id_empresa is not null;



                                        insert into acompanhamento_cct_sindicato_laboral_tb(acompanhamento_cct_id, sindicato_id)
                                        select distinct cct.idacompanhanto_cct acompanhamento_cct_id, sptr.id_sinde sindicato_id
                                        from acompanhanto_cct cct 
                                        inner join sind_emp sptr on cct.sind_emp_id_sinde = sptr.id_sinde or json_contains(cct.ids_sindemp_adicionais, concat('[""', sptr.id_sinde, '""]'));	

                                        insert into acompanhamento_cct_sindicato_patronal_tb(acompanhamento_cct_id, sindicato_id)
                                        select distinct cct.idacompanhanto_cct acompanhamento_cct_id, sptr.id_sindp sindicato_id
                                        from acompanhanto_cct cct 
                                        inner join sind_patr sptr on cct.sind_patr_id_sindp = sptr.id_sindp or json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]'));

                                        insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                        select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                        from acompanhanto_cct cct 
                                        inner join cliente_unidades cut on JSON_CONTAINS(cct.grupos_economicos_ids, concat('[', cut.cliente_grupo_id_grupo_economico, ']'))
                                                                        where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                                                       where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                                                                       and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                                                       and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                                                       and cut.id_unidade = acet.estabelecimento_id);
  
                                        insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                        select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                        from acompanhanto_cct cct 
                                        inner join cliente_unidades cut on JSON_CONTAINS(cct.empresas_ids, concat('[', cut.cliente_matriz_id_empresa, ']'))
                                        where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                       where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                                       and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                       and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                       and cut.id_unidade = acet.estabelecimento_id);		  
                                    END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE if exists inserir_estabelecimentos_sindicatos_acompanhamento;");

            migrationBuilder.Sql(@"CREATE PROCEDURE inserir_estabelecimentos_sindicatos_acompanhamento`()
            BEGIN
	                truncate acompanhamento_cct_estabelecimento_tb;
	                truncate acompanhamento_cct_sindicato_laboral_tb;
	                truncate acompanhamento_cct_sindicato_patronal_tb;
	
	                ALTER TABLE acompanhamento_cct_estabelecimento_tb AUTO_INCREMENT = 1;
	                ALTER TABLE acompanhamento_cct_sindicato_laboral_tb AUTO_INCREMENT = 1;
	                ALTER TABLE acompanhamento_cct_sindicato_patronal_tb AUTO_INCREMENT = 1;
	
	               insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                from acompanhanto_cct cct 
                inner join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde or json_contains(cct.ids_sindemp_adicionais, concat('[""', sempt.id_sinde, '""]'))
                inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
                inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
			                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
			                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
			                   and cut.cliente_matriz_id_empresa = acet.empresa_id
			                   and cut.id_unidade = acet.estabelecimento_id)
			   and cut.cliente_grupo_id_grupo_economico is not null
			   and cut.cliente_matriz_id_empresa is not null;
	
	                insert into acompanhamento_cct_sindicato_laboral_tb(acompanhamento_cct_id, sindicato_id)
	                select distinct cct.idacompanhanto_cct acompanhamento_cct_id, sptr.id_sinde sindicato_id
	                from acompanhanto_cct cct 
	                inner join sind_emp sptr on cct.sind_emp_id_sinde = sptr.id_sinde or json_contains(cct.ids_sindemp_adicionais, concat('[""', sptr.id_sinde, '""]'));
							
	                insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                from acompanhanto_cct cct 
                inner join sind_patr sptr on cct.sind_patr_id_sindp = sptr.id_sindp or json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]'))
                inner join base_territorialsindpatro btspt on sptr.id_sindp = btspt.sind_patronal_id_sindp
                inner join localizacao lt on btspt.localizacao_id_localizacao1 = lt.id_localizacao
                inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
                inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
			                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
			                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
			                   and cut.cliente_matriz_id_empresa = acet.empresa_id
			                   and cut.id_unidade = acet.estabelecimento_id)
              and cut.cliente_grupo_id_grupo_economico is not null
			   and cut.cliente_matriz_id_empresa is not null;
	
	
	                insert into acompanhamento_cct_sindicato_patronal_tb(acompanhamento_cct_id, sindicato_id)
	                select distinct cct.idacompanhanto_cct acompanhamento_cct_id, sptr.id_sindp sindicato_id
	                from acompanhanto_cct cct 
	                inner join sind_patr sptr on cct.sind_patr_id_sindp = sptr.id_sindp or json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]'));
	
	                insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
	                select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
	                from acompanhanto_cct cct 
	                inner join cliente_unidades cut on JSON_CONTAINS(cct.grupos_economicos_ids, concat('[', cut.cliente_grupo_id_grupo_economico, ']'))
	                where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
	                               where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
	                               and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
	                               and cut.cliente_matriz_id_empresa = acet.empresa_id
	                               and cut.id_unidade = acet.estabelecimento_id);
				  
	                insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
	                select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
	                from acompanhanto_cct cct 
	                inner join cliente_unidades cut on JSON_CONTAINS(cct.empresas_ids, concat('[', cut.cliente_matriz_id_empresa, ']'))
	                where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
	                               where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
	                               and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
	                               and cut.cliente_matriz_id_empresa = acet.empresa_id
	                               and cut.id_unidade = acet.estabelecimento_id);		  
                END;
");
        }
    }
}
