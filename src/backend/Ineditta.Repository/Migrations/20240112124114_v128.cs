using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v128 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS `documento_sindical_vw`;");
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW `documento_sindical_vw` AS
                select
                    `dst`.`id_doc` AS `id_doc`,
                    `tdt`.`nome_doc` AS `nome_doc`,
                    `dst`.`data_upload` AS `data_upload`,
                    `dst`.`validade_inicial` AS `validade_inicial`,
                    `dst`.`validade_final` AS `validade_final`,
                    `tdt`.`sigla_doc` AS `sigla_doc`,
                    json_extract(`dst`.`sind_laboral`, '$[*].sigla') AS `sindicatos_laborais_siglas`,
                    json_extract(`dst`.`sind_patronal`, '$[*].sigla') AS `sindicatos_patronais_siglas`,
                    (
                    select
                        json_arrayagg(`gt`.`id`) AS `sindicatos_laborais`
                    from
                        json_table(`dst`.`sind_laboral`, '$[*].id' columns (`id` int path '$')) `gt`) AS `sindicatos_laborais_ids`,
                    `sind_patronais_ids`.`sindicatos_patronais` AS `sindicatos_patronais_ids`,
                    json_extract(`dst`.`sind_laboral`, '$[*].municipio') AS `sindicatos_laborais_municipios`,
                    `dst`.`modulo` AS `modulo`,
                    `dst`.`sind_laboral` AS `sind_laboral`,
                    `dst`.`sind_patronal` AS `sind_patronal`,
                    `dst`.`cliente_estabelecimento` AS `cliente_estabelecimento`,
                    `dst`.`referencia` AS `estrutura_clausulas_ids`,
                    `dst`.`caminho_arquivo` AS `caminho_arquivo`,
                    `dst`.`database_doc` AS `database_doc`,
                    `dst`.`abrangencia` AS `abrangencia`,
                    `dst`.`cnae_doc` AS `cnae_doc`,
                    `dst`.`descricao_documento` AS `descricao_documento`,
                    `tdt`.`tipo_doc` AS `tipo_documento`,
                    `dst`.`anuencia` AS `anuencia`,
                    `dst`.`doc_restrito` AS `doc_restrito`,
                    `dst`.`data_aprovacao` AS `data_aprovacao`,
                    `dst`.`usuario_cadastro` AS `usuario_cadastro`,
                    `dst`.`observacao` AS `observacao`,
                    `tdt`.`processado` AS `processado`,
                    `uat`.`nivel` AS `usuario_cadastro_nivel`,
                    `uat`.`id_grupoecon` AS `usuario_cadastro_grupo_economico`,
                    coalesce(`cgt`.`quantidade_nao_aprovados`, 0) AS `clausula_quantidade_nao_aprovadas`,
                    `cgt`.`data_ultima_aprovacao` AS `clausula_data_ultima_aprovacao`,
                    `tdt`.`idtipo_doc` AS `tipo_doc_id`,
                    `dst`.`data_liberacao_clausulas` AS `data_liberacao_clausulas`,
                    (case
                        when (`dst`.`modulo` = 'SISAP') then cast(`dst`.`data_aprovacao` as date)
                        else cast(`dst`.`data_upload` as date)
                    end) AS `data_inclusao`,
                    `assuntos`.`nomes` assuntos
                from
                    ((((`doc_sind` `dst`
                left join `usuario_adm` `uat` on
                    ((`dst`.`usuario_cadastro` = `uat`.`id_user`)))
                left join lateral (
                    select
                        json_arrayagg(`gt`.`id`) AS `sindicatos_patronais`
                    from
                        json_table(`dst`.`sind_patronal`, '$[*].id' columns (`id` int path '$')) `gt`) `sind_patronais_ids` on
                    (true))
                left join `tipo_doc` `tdt` on
                    ((`dst`.`tipo_doc_idtipo_doc` = `tdt`.`idtipo_doc`)))
                left join lateral (
                    select
                        sum((case when (`cgt`.`aprovado` = 'nao') then 1 else 0 end)) AS `quantidade_nao_aprovados`,
                        max(`cgt`.`data_aprovacao`) AS `data_ultima_aprovacao`
                    from
                        `clausula_geral` `cgt`
                    where
                        (`cgt`.`doc_sind_id_documento` = `dst`.`id_doc`)
                    group by
                        `cgt`.`doc_sind_id_documento`) `cgt` on
                    (true))
                LEFT JOIN LATERAL (
	                SELECT JSON_ARRAYAGG(ec.nome_clausula) nomes FROM estrutura_clausula ec  
	                WHERE JSON_CONTAINS(`dst`.`referencia`, JSON_ARRAY(CAST(ec.id_estruturaclausula AS CHAR)))
                ) assuntos ON true;    
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS `documento_sindical_vw`;");
            migrationBuilder.Sql(@"
                CREATE OR REPLACE `documento_sindical_vw` AS
                select
                    `dst`.`id_doc` AS `id_doc`,
                    `tdt`.`nome_doc` AS `nome_doc`,
                    `dst`.`data_upload` AS `data_upload`,
                    `dst`.`validade_inicial` AS `validade_inicial`,
                    `dst`.`validade_final` AS `validade_final`,
                    `tdt`.`sigla_doc` AS `sigla_doc`,
                    json_extract(`dst`.`sind_laboral`, '$[*].sigla') AS `sindicatos_laborais_siglas`,
                    json_extract(`dst`.`sind_patronal`, '$[*].sigla') AS `sindicatos_patronais_siglas`,
                    (
                    select
                        json_arrayagg(`gt`.`id`) AS `sindicatos_laborais`
                    from
                        json_table(`dst`.`sind_laboral`, '$[*].id' columns (`id` int path '$')) `gt`) AS `sindicatos_laborais_ids`,
                    `sind_patronais_ids`.`sindicatos_patronais` AS `sindicatos_patronais_ids`,
                    json_extract(`dst`.`sind_laboral`, '$[*].municipio') AS `sindicatos_laborais_municipios`,
                    `dst`.`modulo` AS `modulo`,
                    `dst`.`sind_laboral` AS `sind_laboral`,
                    `dst`.`sind_patronal` AS `sind_patronal`,
                    `dst`.`cliente_estabelecimento` AS `cliente_estabelecimento`,
                    `dst`.`referencia` AS `estrutura_clausulas_ids`,
                    `dst`.`caminho_arquivo` AS `caminho_arquivo`,
                    `dst`.`database_doc` AS `database_doc`,
                    `dst`.`abrangencia` AS `abrangencia`,
                    `dst`.`cnae_doc` AS `cnae_doc`,
                    `dst`.`descricao_documento` AS `descricao_documento`,
                    `tdt`.`tipo_doc` AS `tipo_documento`,
                    `dst`.`anuencia` AS `anuencia`,
                    `dst`.`doc_restrito` AS `doc_restrito`,
                    `dst`.`data_aprovacao` AS `data_aprovacao`,
                    `dst`.`usuario_cadastro` AS `usuario_cadastro`,
                    `dst`.`observacao` AS `observacao`,
                    `tdt`.`processado` AS `processado`,
                    `uat`.`nivel` AS `usuario_cadastro_nivel`,
                    `uat`.`id_grupoecon` AS `usuario_cadastro_grupo_economico`,
                    coalesce(`cgt`.`quantidade_nao_aprovados`, 0) AS `clausula_quantidade_nao_aprovadas`,
                    `cgt`.`data_ultima_aprovacao` AS `clausula_data_ultima_aprovacao`,
                    `tdt`.`idtipo_doc` AS `tipo_doc_id`,
                    `dst`.`data_liberacao_clausulas` AS `data_liberacao_clausulas`,
                    (case
                        when (`dst`.`modulo` = 'SISAP') then cast(`dst`.`data_aprovacao` as date)
                        else cast(`dst`.`data_upload` as date)
                    end) AS `data_inclusao`
                from
                    ((((`doc_sind` `dst`
                left join `usuario_adm` `uat` on
                    ((`dst`.`usuario_cadastro` = `uat`.`id_user`)))
                left join lateral (
                    select
                        json_arrayagg(`gt`.`id`) AS `sindicatos_patronais`
                    from
                        json_table(`dst`.`sind_patronal`, '$[*].id' columns (`id` int path '$')) `gt`) `sind_patronais_ids` on
                    (true))
                left join `tipo_doc` `tdt` on
                    ((`dst`.`tipo_doc_idtipo_doc` = `tdt`.`idtipo_doc`)))
                left join lateral (
                    select
                        sum((case when (`cgt`.`aprovado` = 'nao') then 1 else 0 end)) AS `quantidade_nao_aprovados`,
                        max(`cgt`.`data_aprovacao`) AS `data_ultima_aprovacao`
                    from
                        `clausula_geral` `cgt`
                    where
                        (`cgt`.`doc_sind_id_documento` = `dst`.`id_doc`)
                    group by
                        `cgt`.`doc_sind_id_documento`) `cgt` on
                    (true));
            ");
        }
    }
}
