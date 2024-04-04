using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v165 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE inserir_estabelecimentos_acompanhamento;");

            migrationBuilder.Sql(@"CREATE PROCEDURE inserir_estabelecimentos_acompanhamento()
                                    BEGIN
                                        truncate acompanhamento_cct_estabelecimento_tb;

                                        -- Carga Estabelecimentos Laborais Nova
                                        insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                        select distinct cct.id acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                        from acompanhamento_cct_tb cct
                                        inner join lateral (
                                            select sempt.* from acompanhamento_cct_sindicato_laboral_tb acsl
                                            inner join sind_emp sempt on sempt.id_sinde = acsl.sindicato_id
                                            WHERE acsl.sindicato_id = sempt.id_sinde
                                            AND acsl.acompanhamento_cct_id = cct.id
                                        ) as sempt on true
                                        inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                                        inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                                        inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
                                        inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                                        where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                        where cct.id = acet.acompanhamento_cct_id
                                                        and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                        and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                        and cut.id_unidade = acet.estabelecimento_id)
                                        and exists(
                                                select * from acompanhamento_cct_sindicato_patronal_tb acsp 
                                                inner join base_territorialsindpatro btspt on acsp.sindicato_id = btspt.sind_patronal_id_sindp
                                                inner join cnae_emp cet2 on cut.id_unidade = cet2.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet2.classe_cnae_idclasse_cnae
                                                where acsp.acompanhamento_cct_id = cct.id
                                                and cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1)
                                        and exists(
                                                select * from acompanhamento_cct_localizacao_tb aclt
                                                where aclt.acompanhamento_cct_id = cct.id 
            	                                      and cut.localizacao_id_localizacao = aclt.localizacao_id
                                        )
                                        and JSON_CONTAINS(cct.cnaes_ids, JSON_ARRAY(CAST(cet.classe_cnae_idclasse_cnae AS CHAR)))
                                        and cut.cliente_grupo_id_grupo_economico is not null
                                        and cut.cliente_matriz_id_empresa is not null
                                        and (cct.grupos_economicos_ids is null or cct.grupos_economicos_ids = JSON_ARRAY())
                                        and (cct.empresas_ids is null or cct.empresas_ids = JSON_ARRAY());

                                        -- Carga Estabelecimentos Grupo Economicos
                                        insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                        select distinct cct.id acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                        from acompanhamento_cct_tb cct
                                        inner join lateral (
                                            select sempt.* from acompanhamento_cct_sindicato_laboral_tb acsl
                                            inner join sind_emp sempt on sempt.id_sinde = acsl.sindicato_id
                                            WHERE acsl.sindicato_id = sempt.id_sinde
                                            AND acsl.acompanhamento_cct_id = cct.id
                                        ) as sempt on true
                                        inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                                        inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                                        inner join cliente_unidades cut on JSON_CONTAINS(cct.grupos_economicos_ids, concat('[', cut.cliente_grupo_id_grupo_economico, ']')) AND cut.localizacao_id_localizacao = lt.id_localizacao
                                        inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                                        where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                        where cct.id = acet.acompanhamento_cct_id
                                                        and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                        and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                        and cut.id_unidade = acet.estabelecimento_id)
                                        and (exists(
                                                select 1 from acompanhamento_cct_sindicato_patronal_tb acsp 
                                                inner join base_territorialsindpatro btspt on acsp.sindicato_id = btspt.sind_patronal_id_sindp
                                                inner join cnae_emp cet2 on cut.id_unidade = cet2.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet2.classe_cnae_idclasse_cnae
                                                where acsp.acompanhamento_cct_id = cct.id
                                                and cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1)
                                             or not exists (
                                                select 1 from acompanhamento_cct_sindicato_patronal_tb acsp2
                                                where acsp2.acompanhamento_cct_id = cct.id
                                            ))
                                        and exists(
                                                select 1 from acompanhamento_cct_localizacao_tb aclt
                                                where aclt.acompanhamento_cct_id = cct.id 
            	                                      and cut.localizacao_id_localizacao = aclt.localizacao_id
                                        )
                                        and JSON_CONTAINS(cct.cnaes_ids, JSON_ARRAY(CAST(cet.classe_cnae_idclasse_cnae AS CHAR)));

                                        -- Carga Estabelecimentos Matrizes
                                        insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                        select distinct cct.id id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                        from acompanhamento_cct_tb cct 
                                        inner join lateral (
                                            select sempt.* from acompanhamento_cct_sindicato_laboral_tb acsl
                                            inner join sind_emp sempt on sempt.id_sinde = acsl.sindicato_id
                                            WHERE acsl.sindicato_id = sempt.id_sinde
                                            AND acsl.acompanhamento_cct_id = cct.id
                                        ) as sempt on true
                                        inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                                        inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                                        inner join cliente_unidades cut on JSON_CONTAINS(cct.empresas_ids, concat('[', cut.cliente_matriz_id_empresa, ']')) AND cut.localizacao_id_localizacao = lt.id_localizacao
                                        inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                                        where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                        where cct.id = acet.acompanhamento_cct_id
                                                        and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                        and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                        and cut.id_unidade = acet.estabelecimento_id)
                                        and (exists(
                                                select 1 from acompanhamento_cct_sindicato_patronal_tb acsp 
                                                inner join base_territorialsindpatro btspt on acsp.sindicato_id = btspt.sind_patronal_id_sindp
                                                inner join cnae_emp cet2 on cut.id_unidade = cet2.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet2.classe_cnae_idclasse_cnae
                                                where acsp.acompanhamento_cct_id = cct.id
                                                and cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1)
                                             or not exists (
                                                select 1 from acompanhamento_cct_sindicato_patronal_tb acsp2
                                                where acsp2.acompanhamento_cct_id = cct.id
                                            ))
                                        and exists(
                                                select 1 from acompanhamento_cct_localizacao_tb aclt
                                                where aclt.acompanhamento_cct_id = cct.id 
            	                                      and cut.localizacao_id_localizacao = aclt.localizacao_id
                                        )
                                        and JSON_CONTAINS(cct.cnaes_ids, JSON_ARRAY(CAST(cet.classe_cnae_idclasse_cnae AS CHAR)));
                                    END;");

            migrationBuilder.Sql(@"CALL inserir_estabelecimentos_acompanhamento();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE inserir_estabelecimentos_acompanhamento;");

            migrationBuilder.Sql(@"CREATE PROCEDURE inserir_estabelecimentos_acompanhamento()
                                BEGIN
                                    truncate acompanhamento_cct_estabelecimento_tb;

                                    -- Carga Estabelecimentos Laborais Nova
                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                    select distinct cct.id acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                    from acompanhamento_cct_tb cct
                                    inner join lateral (
                                        select sempt.* from acompanhamento_cct_sindicato_laboral_tb acsl
                                        inner join sind_emp sempt on sempt.id_sinde = acsl.sindicato_id
                                        WHERE acsl.sindicato_id = sempt.id_sinde
                                        AND acsl.acompanhamento_cct_id = cct.id
                                    ) as sempt on true
                                    inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                                    inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                                    inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
                                    inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                    where cct.id = acet.acompanhamento_cct_id
                                                    and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                    and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                    and cut.id_unidade = acet.estabelecimento_id)
                                    and exists(
   		                                    select * from acompanhamento_cct_sindicato_patronal_tb acsp 
                                            inner join base_territorialsindpatro btspt on acsp.sindicato_id = btspt.sind_patronal_id_sindp
                                            inner join cnae_emp cet2 on cut.id_unidade = cet2.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet2.classe_cnae_idclasse_cnae
                                            where acsp.acompanhamento_cct_id = cct.id
                                            and cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1)
                                    and cut.cliente_grupo_id_grupo_economico is not null
                                    and cut.cliente_matriz_id_empresa is not null;

                                    -- Carga Estabelecimentos Grupo Economicos
                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                    select distinct cct.id acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                    from acompanhamento_cct_tb cct 
                                    inner join cliente_unidades cut on JSON_CONTAINS(cct.grupos_economicos_ids, concat('[', cut.cliente_grupo_id_grupo_economico, ']'))
                                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                    where cct.id = acet.acompanhamento_cct_id
                                                    and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                    and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                    and cut.id_unidade = acet.estabelecimento_id);

                                    -- Carga Estabelecimentos Matrizes
                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                    select distinct cct.id id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                    from acompanhamento_cct_tb cct 
                                    inner join cliente_unidades cut on JSON_CONTAINS(cct.empresas_ids, concat('[', cut.cliente_matriz_id_empresa, ']'))
                                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                    where cct.id = acet.acompanhamento_cct_id
                                                    and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                    and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                    and cut.id_unidade = acet.estabelecimento_id);
                                END;");

            migrationBuilder.Sql(@"CALL inserir_estabelecimentos_acompanhamento();");
        }
    }
}
