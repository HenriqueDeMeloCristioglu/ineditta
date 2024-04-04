using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v71 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE
                                ALGORITHM = UNDEFINED VIEW `clausula_geral_vw` AS
                                select
                                    `x`.`documento_id` AS `documento_id`,
                                    `x`.`quantidade_processados` AS `quantidade_processados`,
                                    `x`.`quantidade_nao_processados` AS `quantidade_nao_processados`,
                                    `x`.`quantidade_aprovados` AS `quantidade_aprovados`,
                                    `x`.`quantidade_nao_aprovados` AS `quantidade_nao_aprovados`,
                                    `tp`.`nome_doc` AS `nome_doc`,
                                    `ds`.`data_scrap` AS `data_scrap`,
                                    `ds`.`data_aprovacao` AS `data_aprovacao`,
                                    `ds`.`data_sla` AS `data_sla`
                                from
                                    (((
                                    select
                                        `cg`.`doc_sind_id_documento` AS `documento_id`,
                                        sum((case when ((`cg`.`data_processamento` is not null) and (`cg`.`data_processamento` <> '0000-00-00')) then 1 else 0 end)) AS `quantidade_processados`,
                                        sum((case when ((`cg`.`data_processamento` is null) or (`cg`.`data_processamento` = '0000-00-00')) then 1 else 0 end)) AS `quantidade_nao_processados`,
                                        sum((case when ((`cg`.`data_aprovacao` is not null) and (`cg`.`data_aprovacao` <> '0000-00-00')) then 1 else 0 end)) AS `quantidade_aprovados`,
                                        sum((case when ((`cg`.`data_aprovacao` is null) or (`cg`.`data_aprovacao` = '0000-00-00')) then 1 else 0 end)) AS `quantidade_nao_aprovados`
                                    from
                                        `clausula_geral` `cg`
                                    group by
                                        `cg`.`doc_sind_id_documento`) `x`
                                left join `doc_sind` `ds` on
                                    ((`ds`.`id_doc` = `x`.`documento_id`)))
                                left join `tipo_doc` `tp` on
                                    ((`tp`.`idtipo_doc` = `ds`.`tipo_doc_idtipo_doc`)));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE
                                ALGORITHM = UNDEFINED VIEW `clausula_geral_vw` AS
                                select
                                    `x`.`documento_id` AS `documento_id`,
                                    `x`.`quantidade_processados` AS `quantidade_processados`,
                                    `x`.`quantidade_nao_processados` AS `quantidade_nao_processados`,
                                    `x`.`quantidade_aprovados` AS `quantidade_aprovados`,
                                    `x`.`quantidade_nao_aprovados` AS `quantidade_nao_aprovados`,
                                    `tp`.`nome_doc` AS `nome_doc`,
                                    `ds`.`data_scrap` AS `data_scrap`,
                                    `ds`.`data_aprovacao` AS `data_aprovacao`
                                from
                                    (((
                                    select
                                        `cg`.`doc_sind_id_documento` AS `documento_id`,
                                        sum((case when ((`cg`.`data_processamento` is not null) and (`cg`.`data_processamento` <> '0000-00-00')) then 1 else 0 end)) AS `quantidade_processados`,
                                        sum((case when ((`cg`.`data_processamento` is null) or (`cg`.`data_processamento` = '0000-00-00')) then 1 else 0 end)) AS `quantidade_nao_processados`,
                                        sum((case when ((`cg`.`data_aprovacao` is not null) and (`cg`.`data_aprovacao` <> '0000-00-00')) then 1 else 0 end)) AS `quantidade_aprovados`,
                                        sum((case when ((`cg`.`data_aprovacao` is null) or (`cg`.`data_aprovacao` = '0000-00-00')) then 1 else 0 end)) AS `quantidade_nao_aprovados`
                                    from
                                        `clausula_geral` `cg`
                                    group by
                                        `cg`.`doc_sind_id_documento`) `x`
                                left join `doc_sind` `ds` on
                                    ((`ds`.`id_doc` = `x`.`documento_id`)))
                                left join `tipo_doc` `tp` on
                                    ((`tp`.`idtipo_doc` = `ds`.`tipo_doc_idtipo_doc`)));");
        }
    }
}
