using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v126 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_estabelecimento_tb",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    acompanhamento_cct_id = table.Column<int>(type: "int", nullable: false),
                    grupo_economico_id = table.Column<int>(type: "int", nullable: false),
                    empresa_id = table.Column<int>(type: "int", nullable: false),
                    estabelecimento_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_estabelecimento_x_acompanhamento_cct",
                        column: x => x.acompanhamento_cct_id,
                        principalTable: "acompanhanto_cct",
                        principalColumn: "idacompanhanto_cct",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_estabelecimento_x_cliente_grupo",
                        column: x => x.grupo_economico_id,
                        principalTable: "cliente_grupo",
                        principalColumn: "id_grupo_economico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_estabelecimento_x_cliente_matriz",
                        column: x => x.empresa_id,
                        principalTable: "cliente_matriz",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_estabelecimento_x_cliente_unidades",
                        column: x => x.estabelecimento_id,
                        principalTable: "cliente_unidades",
                        principalColumn: "id_unidade",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_sindicato_laboral_tb",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    acompanhamento_cct_id = table.Column<int>(type: "int", nullable: false),
                    sindicato_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_sindicato_laboral_x_acompanhamento_cct",
                        column: x => x.acompanhamento_cct_id,
                        principalTable: "acompanhanto_cct",
                        principalColumn: "idacompanhanto_cct",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_sindicato_laboral_x_sind_emp",
                        column: x => x.sindicato_id,
                        principalTable: "sind_emp",
                        principalColumn: "id_sinde",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_sindicato_patronal_tb",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    acompanhamento_cct_id = table.Column<int>(type: "int", nullable: false),
                    sindicato_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_sindicato_patronal_x_acompanhamento_cct",
                        column: x => x.acompanhamento_cct_id,
                        principalTable: "acompanhanto_cct",
                        principalColumn: "idacompanhanto_cct",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fkacompanhamento_cct_sindicato_patronal_x_sind_patr",
                        column: x => x.sindicato_id,
                        principalTable: "sind_patr",
                        principalColumn: "id_sindp",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_estabelecimento_tb_empresa_id",
                table: "acompanhamento_cct_estabelecimento_tb",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_estabelecimento_tb_estabelecimento_id",
                table: "acompanhamento_cct_estabelecimento_tb",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_estabelecimento_tb_grupo_economico_id",
                table: "acompanhamento_cct_estabelecimento_tb",
                column: "grupo_economico_id");

            migrationBuilder.CreateIndex(
                name: "ix001_acompanhamento_cct_estabelecimento_tb",
                table: "acompanhamento_cct_estabelecimento_tb",
                column: "acompanhamento_cct_id");

            migrationBuilder.CreateIndex(
                name: "ix002_acompanhamento_cct_estabelecimento_tb",
                table: "acompanhamento_cct_estabelecimento_tb",
                columns: new[] { "acompanhamento_cct_id", "estabelecimento_id", "grupo_economico_id", "empresa_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_sindicato_laboral_tb_sindicato_id",
                table: "acompanhamento_cct_sindicato_laboral_tb",
                column: "sindicato_id");

            migrationBuilder.CreateIndex(
                name: "ix001_acompanhamento_cct_sindicato_laboral_tb",
                table: "acompanhamento_cct_sindicato_laboral_tb",
                column: "acompanhamento_cct_id");

            migrationBuilder.CreateIndex(
                name: "ix002_acompanhamento_cct_sindicato_laboral_tb",
                table: "acompanhamento_cct_sindicato_laboral_tb",
                columns: new[] { "acompanhamento_cct_id", "sindicato_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_sindicato_patronal_tb_sindicato_id",
                table: "acompanhamento_cct_sindicato_patronal_tb",
                column: "sindicato_id");

            migrationBuilder.CreateIndex(
                name: "ix001_acompanhamento_cct_sindicato_patronal_tb",
                table: "acompanhamento_cct_sindicato_patronal_tb",
                column: "acompanhamento_cct_id");

            migrationBuilder.CreateIndex(
                name: "ix002_acompanhamento_cct_sindicato_patronal_tb",
                table: "acompanhamento_cct_sindicato_patronal_tb",
                columns: new[] { "acompanhamento_cct_id", "sindicato_id" },
                unique: true);

            migrationBuilder.Sql(@"
                insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                from acompanhanto_cct cct 
                inner join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde or json_contains(cct.ids_sindemp_adicionais, concat('[""', sempt.id_sinde, '""]'))
                inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
								                and json_contains(cut.cnae_unidade, concat('{""id"":', btset.classe_cnae_idclasse_cnae, '}'))
                and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
			                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
			                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
			                   and cut.cliente_matriz_id_empresa = acet.empresa_id
			                   and cut.id_unidade = acet.estabelecimento_id);

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
								                and json_contains(cut.cnae_unidade, concat('{""id"":', btspt.classe_cnae_idclasse_cnae, '}'))
                and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
			                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
			                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
			                   and cut.cliente_matriz_id_empresa = acet.empresa_id
			                   and cut.id_unidade = acet.estabelecimento_id);


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
            ");

            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_relatorio_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_relatorio_vw AS 
                select 
                  cct.idacompanhanto_cct AS id, 
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
                  date_format(
                    cct.ultima_atualizacao, '%d/%m/%Y'
                  ) AS ultima_atualizacao, 
                  ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior, 
                  ir.indicador AS indicador, 
                  ir.dado_real AS dado_real, 
                  cct.ids_cnaes AS ids_cnaes, 
                  date_format(
                    (
                      ir.periodo_data + interval 1 month
                    ), 
                    '%b/%Y'
                  ) AS ir_periodo, 
                  cnaet.atividades_economicas AS atividades_economicas, 
                  acet.estabelecimentos estabelecimentos,
                  sind_ads_laborais.sindicatos AS sindicatos_laborais_adicionais,
                  sind_ads_patronais.sindicatos AS sindicatos_patronais_adicionais
                from acompanhanto_cct cct 
                left join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde
                left join sind_patr spt on cct.sind_patr_id_sindp = spt.id_sindp
                left join tipo_doc tdt on cct.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                left join indecon_real ir on ir.periodo_data is not null 
						                 and cct.data_base = ConvertMonthYear_ENtoPT(
                                              date_format(
                                                (
                                                  ir.periodo_data + interval 1 month
                                                ), 
                                                '%b/%Y'
                                              )
                                            ) and ir.indicador = 'INPC'
                left join lateral (
                                      select 
                                        group_concat(
                                          cnaet.descricao_subclasse separator ', '
                                        ) AS atividades_economicas 
                                      from 
                                        classe_cnae cnaet 
                                      where 
                                        json_contains(
                                          cct.ids_cnaes, 
                                          concat(
                                            '[""', 
                                            cast(
                                              cnaet.id_cnae as char charset utf8mb4
                                            ), 
                                            '""]'
                                          )
                                        )
                                    ) cnaet on (true)
                    
                                  left join lateral (
                                    select 
                                      json_arrayagg(
                                        json_object(
                                          'nome', cut.nome_unidade, 'cnpj', 
                                          cut.cnpj_unidade, 'grupoEconomicoId', 
                                          cut.cliente_grupo_id_grupo_economico, 
                                          'codigoSindicatoCliente', cut.cod_sindcliente
                                        )
                                      ) AS estabelecimentos 
                                      from acompanhamento_cct_estabelecimento_tb acet
                                      inner join cliente_unidades cut on acet.estabelecimento_id = cut.id_unidade
                                      where acet.acompanhamento_cct_id = cct.idacompanhanto_cct
                                      )
                                      acet on true
                left join lateral (
                    select
                        json_arrayagg(json_object('sigla', se.sigla_sinde, 'cnpj', se.cnpj_sinde, 'uf', se.uf_sinde)) AS sindicatos
                    from
                        sind_emp se
                    inner join acompanhamento_cct_sindicato_laboral_tb acspt 
    		                on acspt.acompanhamento_cct_id = cct.idacompanhanto_cct
    		                and acspt.sindicato_id = se.id_sinde
    		                and sempt.id_sinde <> acspt.sindicato_id) sind_ads_laborais on true
                left join lateral (
                    select
                        json_arrayagg(json_object('sigla', sp.sigla_sp, 'cnpj', sp.cnpj_sp, 'uf', sp.uf_sp)) AS sindicatos
                    from
                        sind_patr sp
                        inner join acompanhamento_cct_sindicato_patronal_tb acspt 
    		                on acspt.acompanhamento_cct_id = cct.idacompanhanto_cct
    		                and acspt.sindicato_id = sp.id_sindp
    		                and spt.id_sindp <> acspt.sindicato_id) sind_ads_patronais on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acompanhamento_cct_estabelecimento_tb");

            migrationBuilder.DropTable(
                name: "acompanhamento_cct_sindicato_laboral_tb");

            migrationBuilder.DropTable(
                name: "acompanhamento_cct_sindicato_patronal_tb");

            migrationBuilder.CreateTable(
                name: "ClienteGrupo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LogoGrupo = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteGrupo", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddForeignKey(
                name: "cliente_matriz_ibfk_1",
                table: "cliente_matriz",
                column: "cliente_grupo_id_grupo_economico",
                principalTable: "ClienteGrupo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_informacao_adicional_cliente_tb_x_cliente_grupo",
                table: "informacao_adicional_cliente_tb",
                column: "grupo_economico_id",
                principalTable: "ClienteGrupo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_relatorio_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                    ALGORITHM = UNDEFINED VIEW acompanhamento_cct_relatorio_vw AS
                    select
                        cct.idacompanhanto_cct AS id,
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
                        cnaet.atividades_economicas AS atividades_economicas,
                        estabelecimentos_grupo.estabelecimentos AS estabelecimentos_grupo_economico,
                        estabelecimentos_matriz.estabelecimentos AS estabelecimentos_matriz,
                        esindemp.estabelecimentos AS estabelecimentos_laborais_principais,
                        esindpt.estabelecimentos AS estabelecimentos_patronais_principais,
                        esindempadct.estabelecimentos AS estabelecimentos_laborais_adicionais,
                        esindptadct.estabelecimentos AS estabelecimentos_patronais_adicionais,
                        sind_ads_laborais.sindicatos AS sindicatos_laborais_adicionais,
                        sind_ads_patronais.sindicatos AS sindicatos_patronais_adicionais
                    from
                        (((((((((((((acompanhanto_cct cct
                    left join sind_emp sempt on
                        ((cct.sind_emp_id_sinde = sempt.id_sinde)))
                    left join sind_patr spt on
                        ((cct.sind_patr_id_sindp = spt.id_sindp)))
                    left join tipo_doc tdt on
                        ((cct.tipo_doc_idtipo_doc = tdt.idtipo_doc)))
                    left join indecon_real ir on
                        (((ir.periodo_data is not null)
                            and (cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y')))
                                and (ir.indicador = 'INPC'))))
                    left join lateral (
                        select
                            group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas
                        from
                            classe_cnae cnaet
                        where
                            json_contains(cct.ids_cnaes,
                            concat('[""', cast(cnaet.id_cnae as char charset utf8mb4), '""]'))) cnaet on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('nome',
                            cut.nome_unidade,
                            'cnpj',
                            cut.cnpj_unidade,
                            'grupoEconomicoId',
                            cut.cliente_grupo_id_grupo_economico,
                            'codigoSindicatoCliente',
                            cut.cod_sindcliente)) AS estabelecimentos
                        from
                            ((cliente_unidades cut
                        join localizacao lt on
                            ((cut.localizacao_id_localizacao = lt.id_localizacao)))
                        join base_territorialsindemp btset on
                            (((lt.id_localizacao = btset.localizacao_id_localizacao1)
                                and json_contains(cut.cnae_unidade,
                                concat('{""id"":', btset.classe_cnae_idclasse_cnae, '}'))
                                    and (sempt.id_sinde = btset.sind_empregados_id_sinde1))))) esindemp on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('nome',
                            cut.nome_unidade,
                            'cnpj',
                            cut.cnpj_unidade,
                            'grupoEconomicoId',
                            cut.cliente_grupo_id_grupo_economico,
                            'codigoSindicatoCliente',
                            cut.cod_sindcliente)) AS estabelecimentos
                        from
                            ((cliente_unidades cut
                        join localizacao lt on
                            ((cut.localizacao_id_localizacao = lt.id_localizacao)))
                        join base_territorialsindpatro bt on
                            (((lt.id_localizacao = bt.localizacao_id_localizacao1)
                                and json_contains(cut.cnae_unidade,
                                concat('{""id"":', bt.classe_cnae_idclasse_cnae, '}'))
                                    and (spt.id_sindp = bt.sind_patronal_id_sindp))))) esindpt on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('nome',
                            cut.nome_unidade,
                            'cnpj',
                            cut.cnpj_unidade,
                            'grupoEconomicoId',
                            cut.cliente_grupo_id_grupo_economico,
                            'codigoSindicatoCliente',
                            cut.cod_sindcliente)) AS estabelecimentos
                        from
                            ((cliente_unidades cut
                        join localizacao lt on
                            ((cut.localizacao_id_localizacao = lt.id_localizacao)))
                        join base_territorialsindemp btset on
                            (((lt.id_localizacao = btset.localizacao_id_localizacao1)
                                and json_contains(cut.cnae_unidade,
                                concat('{""id"":', btset.classe_cnae_idclasse_cnae, '}'))
                                    and json_contains(cct.ids_sindemp_adicionais,
                                    json_array(btset.sind_empregados_id_sinde1)))))) esindempadct on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('nome',
                            cut.nome_unidade,
                            'cnpj',
                            cut.cnpj_unidade,
                            'grupoEconomicoId',
                            cut.cliente_grupo_id_grupo_economico,
                            'codigoSindicatoCliente',
                            cut.cod_sindcliente)) AS estabelecimentos
                        from
                            ((cliente_unidades cut
                        join localizacao lt on
                            ((cut.localizacao_id_localizacao = lt.id_localizacao)))
                        join base_territorialsindpatro bt on
                            (((lt.id_localizacao = bt.localizacao_id_localizacao1)
                                and json_contains(cut.cnae_unidade,
                                concat('{""id"":', bt.classe_cnae_idclasse_cnae, '}'))
                                    and json_contains(cct.ids_sindpatr_adicionais,
                                    json_array(bt.sind_patronal_id_sindp)))))) esindptadct on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('sigla',
                            se.sigla_sinde,
                            'cnpj',
                            se.cnpj_sinde,
                            'uf',
                            se.uf_sinde)) AS sindicatos
                        from
                            sind_emp se
                        where
                            json_contains(cct.ids_sindemp_adicionais,
                            json_array(se.id_sinde))) sind_ads_laborais on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('sigla',
                            sp.sigla_sp,
                            'cnpj',
                            sp.cnpj_sp,
                            'uf',
                            sp.uf_sp)) AS sindicatos
                        from
                            sind_patr sp
                        where
                            json_contains(cct.ids_sindpatr_adicionais,
                            json_array(sp.id_sindp))) sind_ads_patronais on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('nome',
                            cut.nome_unidade,
                            'cnpj',
                            cut.cnpj_unidade,
                            'grupoEconomicoId',
                            cut.cliente_grupo_id_grupo_economico,
                            'codigoSindicatoCliente',
                            cut.cod_sindcliente)) AS estabelecimentos
                        from
                            cliente_unidades cut
                        where
                            json_contains(cct.grupos_economicos_ids,
                            json_array(cut.cliente_grupo_id_grupo_economico))) estabelecimentos_grupo on
                        (true))
                    left join lateral (
                        select
                            json_arrayagg(json_object('nome',
                            cut.nome_unidade,
                            'cnpj',
                            cut.cnpj_unidade,
                            'grupoEconomicoId',
                            cut.cliente_grupo_id_grupo_economico,
                            'codigoSindicatoCliente',
                            cut.cod_sindcliente)) AS estabelecimentos
                        from
                            cliente_unidades cut
                        where
                            json_contains(cct.empresas_ids,
                            json_array(cut.cliente_matriz_id_empresa))) estabelecimentos_matriz on
                        (true));");
        }
    }
}
