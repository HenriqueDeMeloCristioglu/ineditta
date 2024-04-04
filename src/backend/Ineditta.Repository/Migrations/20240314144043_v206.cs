using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v206 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status",
                table: "acompanhamento_cct_tb");

            migrationBuilder.DropTable(
                name: "acompanhamento_cct_status_tb");

            migrationBuilder.DropIndex(
                name: "IX_acompanhamento_cct_tb_etiqueta_id",
                table: "acompanhamento_cct_tb");

            migrationBuilder.DropColumn(
                name: "etiqueta_id",
                table: "acompanhamento_cct_tb");

            migrationBuilder.DropColumn(
                name: "descricao",
                table: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.AddColumn<long>(
                name: "acompanhamento_cct_etiqueta_opcao_id",
                table: "acompanhamento_cct_etiqueta_tb",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_etiqueta_tb",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_assunto_tb",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_etiqueta_opcao_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acompanhamento_cct_etiqueta_opcao_tb", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_status_opcao_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acompanhamento_cct_status_opcao_tb", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_etiqueta_tb_acompanhamento_cct_etiqueta_o~",
                table: "acompanhamento_cct_etiqueta_tb",
                column: "acompanhamento_cct_etiqueta_opcao_id");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_etiqueta_tb_acompanhamento_cct_id",
                table: "acompanhamento_cct_etiqueta_tb",
                column: "acompanhamento_cct_id");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhamento_cct_etiqueta_x_acompanhamento_cct",
                table: "acompanhamento_cct_etiqueta_tb",
                column: "acompanhamento_cct_id",
                principalTable: "acompanhamento_cct_tb",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhamento_cct_etiqueta_x_acompanhamento_cct_etiqueta_opcao",
                table: "acompanhamento_cct_etiqueta_tb",
                column: "acompanhamento_cct_etiqueta_opcao_id",
                principalTable: "acompanhamento_cct_etiqueta_opcao_tb",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"insert into acompanhamento_cct_status_opcao_tb (id, descricao)
                                values (1, '1 Cliente'), (2, '2 Alta'), (3, '3 Média'), (4, '4 Baixa')");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status_opcao",
                table: "acompanhamento_cct_tb",
                column: "status",
                principalTable: "acompanhamento_cct_status_opcao_tb",
                principalColumn: "id");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_inclusao_vw AS
                                select
                                    acct.id AS id,
                                    acct.data_inicial AS data_inicial,
                                    acct.data_final AS data_final,
                                    acct.data_alteracao AS data_alteracao,
                                    stcct.descricao AS status,
                                    ua.nome_usuario AS nome_usuario,
                                    fcct.fase_negociacao AS fase,
                                    td.nome_doc AS nome_documento,
                                    acct.proxima_ligacao AS proxima_ligacao,
                                    acct.data_base AS data_base,
                                    sindicatos_patronais.siglas AS sigla_sindicato_patronal,
                                    sindicatos_patronais.ufs AS uf_sindicato_patronal,
                                    sindicatos_laborais.siglas AS sigla_sindicato_empregado,
                                    sindicatos_laborais.ufs AS uf_sindicato_empregado,
                                    cnaes.descricao_subclasse AS descricao_sub_classe,
                                    etiquetas.s etiquetas
                                from acompanhamento_cct_tb acct
                                left join usuario_adm ua on acct.usuario_responsavel_id = ua.id_user
                                left join fase_cct fcct on acct.fase_id = fcct.id_fase
                                left join acompanhamento_cct_status_opcao_tb stcct on acct.status = stcct.id
                                left join tipo_doc td on acct.tipo_documento_id = td.idtipo_doc
                                left join lateral (
                                    select
                                        group_concat(se.id_sinde separator ', ') AS ids,
                                        group_concat(se.sigla_sinde separator ', ') AS siglas,
                                        group_concat(se.cnpj_sinde separator ', ') AS cnpjs,
                                        group_concat(se.uf_sinde separator ', ') AS ufs,
                                        group_concat(se.codigo_sinde separator ', ') AS codigos
                                    from acompanhamento_cct_sindicato_laboral_tb acslt
                                    join sind_emp se on acslt.sindicato_id = se.id_sinde
                                    where acslt.acompanhamento_cct_id = acct.id
                                ) sindicatos_laborais on true
                                left join lateral (
                                    select
                                        group_concat(sp.id_sindp separator ', ') AS ids,
                                        group_concat(sp.sigla_sp separator ', ') AS siglas,
                                        group_concat(sp.cnpj_sp separator ', ') AS cnpjs,
                                        group_concat(sp.uf_sp separator ', ') AS ufs,
                                        group_concat(sp.codigo_sp separator ', ') AS codigos
                                    from acompanhamento_cct_sindicato_patronal_tb acspt
                                    join sind_patr sp on acspt.sindicato_id = sp.id_sindp
                                    where acspt.acompanhamento_cct_id = acct.id
                                ) sindicatos_patronais on true
                                left join lateral (
                                    select group_concat(ccnae.descricao_subclasse separator ', ') AS descricao_subclasse
                                    from classe_cnae ccnae
                                    where json_contains(acct.cnaes_ids, json_array(cast(ccnae.id_cnae as char charset utf8mb4)))
                                ) cnaes on true
                                LEFT JOIN lateral (
                                    select GROUP_CONCAT(aceot.descricao SEPARATOR ',') as s
                                    from acompanhamento_cct_etiqueta_tb acet
                                    inner join acompanhamento_cct_etiqueta_opcao_tb aceot on acet.acompanhamento_cct_etiqueta_opcao_id = aceot.id
                                    where acet.acompanhamento_cct_id = acct.id
                                ) as etiquetas on true
                                order by acct.proxima_ligacao desc, acct.status");

            migrationBuilder.Sql(@"insert into acompanhamento_cct_etiqueta_opcao_tb (descricao)
                                values ('Alteração de representação'),
                                ('CCT homologada pela Federação'),
                                ('Contato bom'),
                                ('Contato ruim'),
                                ('Até 10 clientes'),
                                ('10 à 20 clientes'),
                                ('Mais de 20 clientes'),
                                ('Reenquadramento')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"truncate acompanhamento_cct_etiqueta_opcao_tb");

            migrationBuilder.DropForeignKey(
                name: "fk_acompanhamento_cct_etiqueta_x_acompanhamento_cct",
                table: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.DropForeignKey(
                name: "fk_acompanhamento_cct_etiqueta_x_acompanhamento_cct_etiqueta_opcao",
                table: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.DropForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status_opcao",
                table: "acompanhamento_cct_tb");

            migrationBuilder.DropTable(
                name: "acompanhamento_cct_etiqueta_opcao_tb");

            migrationBuilder.DropTable(
                name: "acompanhamento_cct_status_opcao_tb");

            migrationBuilder.DropIndex(
                name: "IX_acompanhamento_cct_etiqueta_tb_acompanhamento_cct_etiqueta_o~",
                table: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.DropIndex(
                name: "IX_acompanhamento_cct_etiqueta_tb_acompanhamento_cct_id",
                table: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.DropColumn(
                name: "acompanhamento_cct_etiqueta_opcao_id",
                table: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.DropColumn(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.AddColumn<long>(
                name: "etiqueta_id",
                table: "acompanhamento_cct_tb",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "acompanhamento_cct_etiqueta_tb",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_assunto_tb",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_status_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acompanhamento_cct_status_tb", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_tb_etiqueta_id",
                table: "acompanhamento_cct_tb",
                column: "etiqueta_id");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_etiqueta",
                table: "acompanhamento_cct_tb",
                column: "etiqueta_id",
                principalTable: "acompanhamento_cct_etiqueta_tb",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status",
                table: "acompanhamento_cct_tb",
                column: "status",
                principalTable: "acompanhamento_cct_status_tb",
                principalColumn: "id");

            migrationBuilder.Sql(@"insert into acompanhamento_cct_status_tb (id, descricao)
                                values (1, '1 Cliente'), (2, '2 Alta'), (3, '3 Média'), (4, '4 Baixa')");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_inclusao_vw AS
                        select
                            acct.id AS id,
                            acct.data_inicial AS data_inicial,
                            acct.data_final AS data_final,
                            acct.data_alteracao AS data_alteracao,
                            stcct.descricao AS status,
                            ua.nome_usuario AS nome_usuario,
                            fcct.fase_negociacao AS fase,
                            td.nome_doc AS nome_documento,
                            acct.proxima_ligacao AS proxima_ligacao,
                            acct.data_base AS data_base,
                            sindicatos_patronais.siglas AS sigla_sindicato_patronal,
                            sindicatos_patronais.ufs AS uf_sindicato_patronal,
                            sindicatos_laborais.siglas AS sigla_sindicato_empregado,
                            sindicatos_laborais.ufs AS uf_sindicato_empregado,
                            cnaes.descricao_subclasse AS descricao_sub_classe
                        from
                            (((((((acompanhamento_cct_tb acct
                        left join usuario_adm ua on
                            ((acct.usuario_responsavel_id = ua.id_user)))
                        left join fase_cct fcct on
                            ((acct.fase_id = fcct.id_fase)))
                        left join acompanhamento_cct_status_tb stcct on
                            ((acct.status = stcct.id)))
                        left join tipo_doc td on
                            ((acct.tipo_documento_id = td.idtipo_doc)))
                        left join lateral (
                            select
                                group_concat(se.id_sinde separator ', ') AS ids,
                                group_concat(se.sigla_sinde separator ', ') AS siglas,
                                group_concat(se.cnpj_sinde separator ', ') AS cnpjs,
                                group_concat(se.uf_sinde separator ', ') AS ufs,
                                group_concat(se.codigo_sinde separator ', ') AS codigos
                            from
                                (acompanhamento_cct_sindicato_laboral_tb acslt
                            join sind_emp se on
                                ((acslt.sindicato_id = se.id_sinde)))
                            where
                                (acslt.acompanhamento_cct_id = acct.id)) sindicatos_laborais on
                            (true))
                        left join lateral (
                            select
                                group_concat(sp.id_sindp separator ', ') AS ids,
                                group_concat(sp.sigla_sp separator ', ') AS siglas,
                                group_concat(sp.cnpj_sp separator ', ') AS cnpjs,
                                group_concat(sp.uf_sp separator ', ') AS ufs,
                                group_concat(sp.codigo_sp separator ', ') AS codigos
                            from
                                (acompanhamento_cct_sindicato_patronal_tb acspt
                            join sind_patr sp on
                                ((acspt.sindicato_id = sp.id_sindp)))
                            where
                                (acspt.acompanhamento_cct_id = acct.id)) sindicatos_patronais on
                            (true))
                        left join lateral (
                            select
                                group_concat(ccnae.descricao_subclasse separator ', ') AS descricao_subclasse
                            from
                                classe_cnae ccnae
                            where
                                json_contains(acct.cnaes_ids, json_array(cast(ccnae.id_cnae as char charset utf8mb4)))) cnaes on
                            (true))
                        order by
                            acct.proxima_ligacao desc,
                            acct.status;");
        }
    }
}
