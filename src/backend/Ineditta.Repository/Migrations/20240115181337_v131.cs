using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v131 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE inserir_estabelecimentos_sindicatos_acompanhamento;");

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
                                                                where cct.sind_patr_id_sindp = sptr.id_sindp
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
                                                            where json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]'))
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

            migrationBuilder.Sql(@"call inserir_estabelecimentos_sindicatos_acompanhamento();");

            migrationBuilder.Sql(@"
                update fase_cct fct set fct.prioridade = 1 WHERE fct.prioridade = 'Alta';
                update fase_cct fct set fct.prioridade = 2 WHERE fct.prioridade = 'Média';
                update fase_cct fct set fct.prioridade = 3 WHERE fct.prioridade = 'Baixa';

                update fase_cct fct set fct.periodicidade = 1 WHERE fct.periodicidade = 'Semanal';
                update fase_cct fct set fct.periodicidade = 2 WHERE fct.periodicidade = 'Mensal';
                update fase_cct fct set fct.periodicidade = 3 WHERE fct.periodicidade = 'Quinzenal';
                update fase_cct fct set fct.periodicidade = null WHERE fct.periodicidade = '';

                update fase_cct fct set fct.tipo_fase = 1 WHERE fct.tipo_fase = 'cct';
                update fase_cct fct set fct.tipo_fase = 2 WHERE fct.tipo_fase = 'ccts';

                update acompanhanto_cct acct set acct.fase = 1 WHERE acct.fase = 'Assembleia Patronal';
                update acompanhanto_cct acct set acct.fase = 2 WHERE acct.fase = 'Negociação Não iniciada';
                update acompanhanto_cct acct set acct.fase = 3 WHERE acct.fase = 'Em Negociação';
                update acompanhanto_cct acct set acct.fase = 4 WHERE acct.fase = 'Sem Negociação';
                update acompanhanto_cct acct set acct.fase = 5 WHERE acct.fase = 'Dissídio Coletivo';
                update acompanhanto_cct acct set acct.fase = 6 WHERE acct.fase = 'Fechada-formalização documento';
                update acompanhanto_cct acct set acct.fase = 7 WHERE acct.fase = 'Concluída';
                update acompanhanto_cct acct set acct.fase = 8 WHERE acct.fase = 'Arquivada';
                update acompanhanto_cct acct set acct.fase = 9 WHERE acct.fase = 'Acordo Coletivo Pendente';

                update acompanhanto_cct acct set acct.status = 1 WHERE acct.status = '1 Cliente';
                update acompanhanto_cct acct set acct.status = 2 WHERE acct.status = '2 Alta';
                update acompanhanto_cct acct set acct.status = 3 WHERE acct.status = '3 Média';
                update acompanhanto_cct acct set acct.status = 4 WHERE acct.status = '4 Baixa';

                ALTER TABLE acompanhamento_cct_estabelecimento_tb
                DROP FOREIGN KEY fk_acompanhamento_cct_estabelecimento_x_acompanhamento_cct;

                ALTER TABLE acompanhamento_cct_sindicato_laboral_tb
                DROP FOREIGN KEY fk_acompanhamento_cct_sindicato_laboral_x_acompanhamento_cct;

                ALTER TABLE acompanhamento_cct_sindicato_patronal_tb 
                DROP FOREIGN KEY fk_acompanhamento_cct_sindicato_patronal_x_acompanhamento_cct;
            ");

            migrationBuilder.DropTable(
                name: "acompanhamento_cliente");

            migrationBuilder.DropTable(
                name: "acompanhamento_envolvidos_emp");

            migrationBuilder.DropTable(
                name: "acompanhamento_envolvidos_patr");

            migrationBuilder.DropTable(
                name: "negociacao");

            migrationBuilder.DropIndex(
                name: "fases_cct_id_fases_ifbk2",
                table: "acompanhanto_cct");

            migrationBuilder.Sql(@"ALTER TABLE acompanhanto_cct CHANGE fase fase_id BIGINT NULL;");

            migrationBuilder.DropColumn(
                name: "ids_sindemp_adicionais",
                table: "acompanhanto_cct");

            migrationBuilder.DropColumn(
                name: "ids_sindpatr_adicionais",
                table: "acompanhanto_cct");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "acompanhanto_cct",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE acompanhanto_cct cct set data_alteracao = cct.ultima_atualizacao;");

            migrationBuilder.DropColumn(
                name: "ultima_atualizacao",
                table: "acompanhanto_cct");

            migrationBuilder.AlterTable(
                name: "fase_cct")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "tipo_fase",
                table: "fase_cct",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "prioridade",
                table: "fase_cct",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "periodicidade",
                table: "fase_cct",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "fase_negociacao",
                table: "fase_cct",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<long>(
                name: "id_fase",
                table: "fase_cct",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "status",
                table: "acompanhanto_cct",
                type: "bigint",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<long>(
                name: "idacompanhanto_cct",
                table: "acompanhanto_cct",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inclusao",
                table: "acompanhanto_cct",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "acompanhanto_cct",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "acompanhanto_cct",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_sindicato_patronal_tb",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_sindicato_laboral_tb",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_estabelecimento_tb",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_status",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acompanhamento_cct_status", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.Sql(@"INSERT INTO acompanhamento_cct_status (descricao) values ('1 Cliente'), ('2 Alta'), ('3 Média'), ('4 Baixa');");

            migrationBuilder.CreateIndex(
                name: "fases_cct_id_fases_ifbk2",
                table: "acompanhanto_cct",
                column: "fase_id");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhanto_cct_status",
                table: "acompanhanto_cct",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status",
                table: "acompanhanto_cct",
                column: "status",
                principalTable: "acompanhamento_cct_status",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhanto_cct_x_fase_cct",
                table: "acompanhanto_cct",
                column: "fase_id",
                principalTable: "fase_cct",
                principalColumn: "id_fase");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhanto_cct_x_tipo_doc",
                table: "acompanhanto_cct",
                column: "tipo_doc_idtipo_doc",
                principalTable: "tipo_doc",
                principalColumn: "idtipo_doc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"
                ALTER TABLE acompanhamento_cct_estabelecimento_tb
                ADD CONSTRAINT fk_acompanhamento_cct_estabelecimento_x_acompanhamento_cct
                FOREIGN KEY (acompanhamento_cct_id)
                REFERENCES acompanhanto_cct(idacompanhanto_cct);

                ALTER TABLE acompanhamento_cct_sindicato_laboral_tb
                ADD CONSTRAINT fk_acompanhamento_cct_sindicato_laboral_x_acompanhamento_cct
                FOREIGN KEY (acompanhamento_cct_id)
                REFERENCES acompanhanto_cct(idacompanhanto_cct);

                ALTER TABLE acompanhamento_cct_sindicato_patronal_tb
                ADD CONSTRAINT fk_acompanhamento_cct_sindicato_patronal_x_acompanhamento_cct
                FOREIGN KEY (acompanhamento_cct_id)
                REFERENCES acompanhanto_cct(idacompanhanto_cct);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS inserir_estabelecimentos_sindicatos_acompanhamento;");

            migrationBuilder.Sql(@"CREATE PROCEDURE inserir_estabelecimentos_sindicatos_acompanhamento()
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
                END;");

            migrationBuilder.Sql(@"call inserir_estabelecimentos_sindicatos_acompanhamento();");

            migrationBuilder.Sql(@"
                ALTER TABLE acompanhamento_cct_estabelecimento_tb
                DROP FOREIGN KEY fk_acompanhamento_cct_estabelecimento_x_acompanhamento_cct;

                ALTER TABLE acompanhamento_cct_sindicato_laboral_tb
                DROP FOREIGN KEY fk_acompanhamento_cct_sindicato_laboral_x_acompanhamento_cct;

                ALTER TABLE acompanhamento_cct_sindicato_patronal_tb 
                DROP FOREIGN KEY fk_acompanhamento_cct_sindicato_patronal_x_acompanhamento_cct;
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status",
                table: "acompanhanto_cct");

            migrationBuilder.DropForeignKey(
                name: "fk_acompanhanto_cct_x_fase_cct",
                table: "acompanhanto_cct");

            migrationBuilder.DropForeignKey(
                name: "fk_acompanhanto_cct_x_tipo_doc",
                table: "acompanhanto_cct");

            migrationBuilder.DropTable(
                name: "acompanhamento_cct_status");

            migrationBuilder.DropIndex(
                name: "fases_cct_id_fases_ifbk2",
                table: "acompanhanto_cct");

            migrationBuilder.DropIndex(
                name: "IX_acompanhanto_cct_status",
                table: "acompanhanto_cct");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ultima_atualizacao",
                table: "acompanhanto_cct",
                type: "date",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE acompanhanto_cct cct set ultima_atualizacao = cct.data_alteracao;");

            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "acompanhanto_cct");

            migrationBuilder.DropColumn(
                name: "data_inclusao",
                table: "acompanhanto_cct");

            migrationBuilder.Sql(@"ALTER TABLE acompanhanto_cct CHANGE fase_id fase TEXT NULL;");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "acompanhanto_cct");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "acompanhanto_cct");

            migrationBuilder.AlterTable(
                name: "fase_cct")
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tipo_fase",
                table: "fase_cct",
                type: "varchar(15)",
                maxLength: 15,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AlterColumn<string>(
                name: "prioridade",
                table: "fase_cct",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AlterColumn<string>(
                name: "periodicidade",
                table: "fase_cct",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AlterColumn<string>(
                name: "fase_negociacao",
                table: "fase_cct",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<int>(
                name: "id_fase",
                table: "fase_cct",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.Sql(@"
                update fase_cct fct set fct.prioridade = 'Alta' WHERE fct.prioridade = 1;
                update fase_cct fct set fct.prioridade = 'Média' WHERE fct.prioridade = 2;
                update fase_cct fct set fct.prioridade = 'Baixa' WHERE fct.prioridade = 3;

                update fase_cct fct set fct.periodicidade = 'Semanal' WHERE fct.periodicidade = 1;
                update fase_cct fct set fct.periodicidade = 'Mensal' WHERE fct.periodicidade = 2;
                update fase_cct fct set fct.periodicidade = 'Quinzenal' WHERE fct.periodicidade = 3;
                update fase_cct fct set fct.periodicidade = '' WHERE fct.periodicidade = null;

                update fase_cct fct set fct.tipo_fase = 1 WHERE fct.tipo_fase = 'cct';
                update fase_cct fct set fct.tipo_fase = 2 WHERE fct.tipo_fase = 'ccts';");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "acompanhanto_cct",
                type: "varchar(15)",
                maxLength: 15,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldMaxLength: 15,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
                ALTER TABLE acompanhanto_cct CHANGE fase_id fase TEXT NULL;

                update acompanhanto_cct acct set acct.fase = 1 WHERE acct.fase = 'Assembleia Patronal';
                update acompanhanto_cct acct set acct.fase = 2 WHERE acct.fase = 'Negociação Não iniciada';
                update acompanhanto_cct acct set acct.fase = 3 WHERE acct.fase = 'Em Negociação';
                update acompanhanto_cct acct set acct.fase = 4 WHERE acct.fase = 'Sem Negociação';
                update acompanhanto_cct acct set acct.fase = 5 WHERE acct.fase = 'Dissídio Coletivo';
                update acompanhanto_cct acct set acct.fase = 6 WHERE acct.fase = 'Fechada-formalização documento';
                update acompanhanto_cct acct set acct.fase = 7 WHERE acct.fase = 'Concluída';
                update acompanhanto_cct acct set acct.fase = 8 WHERE acct.fase = 'Arquivada';
                update acompanhanto_cct acct set acct.fase = 9 WHERE acct.fase = 'Acordo Coletivo Pendente';

                update acompanhanto_cct acct set acct.status = 1 WHERE acct.status = '1 Cliente';
                update acompanhanto_cct acct set acct.status = 2 WHERE acct.status = '2 Alta';
                update acompanhanto_cct acct set acct.status = 3 WHERE acct.status = '3 Média';
                update acompanhanto_cct acct set acct.status = 4 WHERE acct.status = '4 Baixa';
            ");

            migrationBuilder.AlterColumn<int>(
                name: "idacompanhanto_cct",
                table: "acompanhanto_cct",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "ids_sindemp_adicionais",
                table: "acompanhanto_cct",
                type: "json",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ids_sindpatr_adicionais",
                table: "acompanhanto_cct",
                type: "json",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_sindicato_patronal_tb",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_sindicato_laboral_tb",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "acompanhamento_cct_id",
                table: "acompanhamento_cct_estabelecimento_tb",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cliente",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_acompanhamento_cct = table.Column<int>(type: "int", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    comentario = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    criado_em = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fase = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "acompanhamento_cct",
                        column: x => x.id_acompanhamento_cct,
                        principalTable: "acompanhanto_cct",
                        principalColumn: "idacompanhanto_cct",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "acompanhamento_envolvidos_emp",
                columns: table => new
                {
                    id_acompanhamento_envolvidos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    acompanhamento_cct_idacompanhamento_cct = table.Column<int>(type: "int", nullable: false),
                    sind_emp_id_sinde = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_acompanhamento_envolvidos);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "acompanhamento_envolvidos_patr",
                columns: table => new
                {
                    id_acompanhamento_envolvidos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    acompanhamento_cct_idacompanhamento_cct = table.Column<int>(type: "int", nullable: false),
                    sind_patr_id_sindp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_acompanhamento_envolvidos);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "negociacao",
                columns: table => new
                {
                    idnegociacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cliente_matriz_id_empresa = table.Column<int>(type: "int", nullable: true),
                    cliente_unidades_id_unidade = table.Column<int>(type: "int", nullable: true),
                    data_abertura = table.Column<DateOnly>(type: "date", nullable: true),
                    data_base = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    fase_cct_id_fase = table.Column<int>(type: "int", nullable: false),
                    id_acompanhamento_cct = table.Column<int>(type: "int", nullable: true),
                    sind_emp_id_sinde = table.Column<int>(type: "int", nullable: false),
                    sind_patr_id_sindp = table.Column<int>(type: "int", nullable: true),
                    tipo_doc_idtipo_doc = table.Column<int>(type: "int", nullable: false),
                    usuario_adm_id_user = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.idnegociacao);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateIndex(
                name: "fases_cct_id_fases_ifbk2",
                table: "acompanhanto_cct",
                column: "fase");

            migrationBuilder.CreateIndex(
                name: "acompanhamento_cct",
                table: "acompanhamento_cliente",
                column: "id_acompanhamento_cct");

            migrationBuilder.Sql(@"
                ALTER TABLE acompanhamento_cct_estabelecimento_tb
                ADD CONSTRAINT fk_acompanhamento_cct_estabelecimento_x_acompanhamento_cct
                FOREIGN KEY (acompanhamento_cct_id)
                REFERENCES acompanhanto_cct(idacompanhanto_cct);

                ALTER TABLE acompanhamento_cct_sindicato_laboral_tb
                ADD CONSTRAINT fk_acompanhamento_cct_sindicato_laboral_x_acompanhamento_cct
                FOREIGN KEY (acompanhamento_cct_id)
                REFERENCES acompanhanto_cct(idacompanhanto_cct);

                ALTER TABLE acompanhamento_cct_sindicato_patronal_tb
                ADD CONSTRAINT fk_acompanhamento_cct_sindicato_patronal_x_acompanhamento_cct
                FOREIGN KEY (acompanhamento_cct_id)
                REFERENCES acompanhanto_cct(idacompanhanto_cct);
            ");
        }
    }
}
