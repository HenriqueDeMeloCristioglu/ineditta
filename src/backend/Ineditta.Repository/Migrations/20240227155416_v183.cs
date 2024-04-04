using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v183 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clausula_cliente_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    clausula_id = table.Column<int>(type: "int", nullable: false),
                    texto = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    grupo_economico_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clausula_cliente_tb", x => x.id);
                    table.ForeignKey(
                        name: "fk_clausula_cliente_tb_clausula_geral",
                        column: x => x.clausula_id,
                        principalTable: "clausula_geral",
                        principalColumn: "id_clau",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_clausula_cliente_tb_cliente_grupo",
                        column: x => x.grupo_economico_id,
                        principalTable: "cliente_grupo",
                        principalColumn: "id_grupo_economico",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_cliente_tb_clausula_id",
                table: "clausula_cliente_tb",
                column: "clausula_id");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_cliente_tb_grupo_economico_id",
                table: "clausula_cliente_tb",
                column: "grupo_economico_id");

            migrationBuilder.Sql(@"drop procedure obter_clausulas_ultimo_ano_sindicatos_laborais;");

            migrationBuilder.Sql(@"CREATE PROCEDURE obter_clausulas_ultimo_ano_sindicatos_laborais(in usuario_id INT)
                                BEGIN

                                       DECLARE query VARCHAR(1000);
  
	                                   DELETE FROM documento_sindicato_mais_recente_usuario_tb dsmrut WHERE dsmrut.usuario_id = usuario_id;
  
	
                                        CREATE TABLE IF NOT EXISTS documento_sindicato_temp (
                                            documento_sindical_id INT,
                                            sindicato_laboral_id INT,
                                            tipo_documento_id INT,
                                            nome_doc VARCHAR(255),
                                            mes_validade_inicial INT,
                                            ano_validade_inicial INT,
                                            mes_validade_final INT,
                                            ano_validade_final INT,
                                            database_doc VARCHAR(255),
                                            validade_inicial DATE,
                                            validade_final DATE,
                                            sigla VARCHAR(500),
                                            ano_mes_validade_inicial INT,
                                            ano_mes_validade_final INT,
                                            usuario_id int
                                        );
   
                                      CREATE TABLE IF NOT EXISTS documento_sindicato_mais_recente_temp (
                                            sindicato_laboral_id INT,
                                            ano_mes_validade_inicial INT,
                                            ano_mes_validade_final INT,
                                            documento_sindical_id INT,
                                            row_num INT,
                                            usuario_id int
                                        );
           
                                       
                                    CREATE TABLE IF NOT EXISTS documento_sindicato_clausula_temp (
                                        documento_sindical_id INT,
                                        sindicato_laboral_id INT,
                                        tipo_documento_id INT,
                                        nome_doc VARCHAR(255),
                                        mes_validade_inicial INT,
                                        ano_validade_inicial INT,
                                        mes_validade_final INT,
                                        ano_validade_final INT,
                                        database_doc VARCHAR(255),
                                        validade_inicial DATE,
                                        validade_final DATE,
                                        sigla VARCHAR(500),
                                        ano_mes_validade_inicial INT,
                                        ano_mes_validade_final INT,
                                        clausula_geral_id int,
                                        estrutura_clausula_geral_id int,
                                        usuario_id int
                                    );
                                   
                                   DELETE FROM documento_sindicato_temp where usuario_id = usuario_id;
                                   DELETE FROM documento_sindicato_mais_recente_temp where usuario_id = usuario_id;
	                               DELETE FROM documento_sindicato_clausula_temp where usuario_id = usuario_id;
  
	
                                    INSERT INTO documento_sindicato_temp (
                                    documento_sindical_id,
                                    sindicato_laboral_id,
                                    tipo_documento_id,
                                    nome_doc,
                                    mes_validade_inicial,
                                    ano_validade_inicial,
                                    mes_validade_final,
                                    ano_validade_final,
                                    database_doc,
                                    validade_inicial,
                                    validade_final,
                                    sigla,
                                    ano_mes_validade_inicial,
                                    ano_mes_validade_final,
                                    usuario_id
                                    )
                                    select dct.id_doc documento_sindical_id, sempt.id sindicato_laboral_id, dct.tipo_doc_idtipo_doc  tipo_documento_id, tdt.nome_doc, month(dct.validade_inicial) mes_validade_inicial, year(dct.validade_inicial) ano_validade_inicial, month(dct.validade_final) mes_validade_final, year(dct.validade_final) ano_validade_final, 
                                    dct.database_doc, dct.validade_inicial, dct.validade_final, sempt.sigla, year(dct.validade_inicial) *100 + month(dct.validade_inicial) ano_mes_validade_inicial, year(dct.validade_final) * 100 + month(dct.validade_final) ano_mes_validade_final, usuario_id
                                    from doc_sind dct
                                    inner join tipo_doc tdt on tdt.idtipo_doc = dct.tipo_doc_idtipo_doc,
                                    json_table(dct.sind_laboral, '$[*]' columns (
    id int4 path '$.id',
    sigla varchar(500) path '$.sigla',
    codigo varchar(500) path '$.codigo'
                                    )) sempt
                                    where dct.data_liberacao_clausulas is not null
                                   and YEAR(dct.data_liberacao_clausulas) > 1900;

                                    INSERT INTO documento_sindicato_mais_recente_temp (
                                    sindicato_laboral_id,
                                    ano_mes_validade_inicial,
                                    ano_mes_validade_final,
                                    documento_sindical_id,
                                    row_num,
                                    usuario_id
                                    )
                                    select x.*, usuario_id from (
                                            WITH RankedRows AS (
                                                  SELECT
                                                    sindicato_laboral_id,
                                                    ano_mes_validade_inicial,
                                                    ano_mes_validade_final, 
                                                    documento_sindical_id,
                                                    ROW_NUMBER() OVER (PARTITION BY sindicato_laboral_id ORDER BY ano_mes_validade_inicial DESC, ano_mes_validade_final desc) AS row_num
                                                  FROM documento_sindicato_temp
                                                  WHERE tipo_documento_id IN (5, 6)
                                                )
                                                SELECT
                                                  *
                                                FROM RankedRows
                                                WHERE row_num = 1) x;
               
               
                                    insert into documento_sindicato_mais_recente_usuario_tb
    	                                (sindicato_laboral_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, clausula_geral_id, estrutura_clausula_id, usuario_id, row_num)
   	                                select dstt.sindicato_laboral_id, dstt.ano_mes_validade_inicial, dstt.ano_mes_validade_final, dstt.documento_sindical_id, cgt.clausula_id, cgt.estrutura_clausula_id, usuario_id, row_num                                   	
                                    from documento_sindicato_mais_recente_temp dstt
                                    inner join clausulas_vw cgt on dstt.documento_sindical_id = cgt.documento_id
                                    where dstt.row_num = 1
                                   	and cgt.consta_no_documento = 1;
   
                                   insert into documento_sindicato_mais_recente_usuario_tb
    	                                (sindicato_laboral_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, clausula_geral_id, estrutura_clausula_id, usuario_id, row_num)
    	                              SELECT 
									    sindicato_laboral_id,
									    ano_mes_validade_inicial,
									    ano_mes_validade_final,
									    documento_sindical_id,
									    clausula_id,
									    estrutura_clausula_id,
									    usuario_id,
									    row_num
									FROM (
									    SELECT 
									        dstt.sindicato_laboral_id,
									        dstt.ano_mes_validade_inicial,
									        dstt.ano_mes_validade_final,
									        dstt.documento_sindical_id,
									        cgt.clausula_id,
									        cgt.estrutura_clausula_id,
									        usuario_id,
									        ROW_NUMBER() OVER (PARTITION BY cgt.estrutura_clausula_id ORDER BY dstt.sindicato_laboral_id) AS row_num
									    FROM documento_sindicato_temp dstt
									    INNER JOIN documento_sindicato_mais_recente_temp dsmrtt 
									        ON dstt.sindicato_laboral_id = dsmrtt.sindicato_laboral_id
									        AND dstt.ano_mes_validade_inicial BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
									        AND dstt.ano_mes_validade_final BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
									        AND dstt.documento_sindical_id <> dsmrtt.documento_sindical_id
									    INNER JOIN LATERAL (
									        SELECT 
									            cgt.estrutura_clausula_id,
									            MAX(cgt.clausula_id) clausula_id 
									        FROM clausulas_vw cgt 
									        WHERE dstt.documento_sindical_id = cgt.documento_id
		                                   	and cgt.consta_no_documento = 1
									        GROUP BY 1
									    ) cgt ON TRUE
									    WHERE NOT EXISTS (
									        SELECT 1 
									        FROM documento_sindicato_mais_recente_usuario_tb dsct 
									        WHERE dsct.estrutura_clausula_id = cgt.estrutura_clausula_id
									        AND dsct.sindicato_laboral_id = dstt.sindicato_laboral_id
									    )
									) AS subquery
									WHERE row_num = 1;

                                END;");

            migrationBuilder.Sql(@"DROP VIEW IF EXISTS documento_mapa_sindical_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW documento_mapa_sindical_vw AS
                        select
                            dct.id_doc AS documento_id,
                            dct.sind_patronal AS documento_sindicato_patronal,
                            dct.sind_laboral AS documento_sindicato_laboral,
                            dct.data_reg_mte AS data_registro,
                            dct.data_aprovacao AS documento_data_aprovacao,
                            dct.database_doc AS documento_database,
                            dct.tipo_doc_idtipo_doc AS documento_tipo_id,
                            dct.cnae_doc AS documento_cnae,
                            dct.abrangencia AS documento_abrangencia,
                            dct.cliente_estabelecimento AS documento_estabelecimento,
                            dct.validade_inicial AS documento_validade_inicial,
                            dct.validade_final AS documento_validade_final,
                            dct.uf AS documento_uf,
                            dct.descricao_documento AS documento_descricao,
                            dct.titulo_documento AS documento_titulo,
                            td.nome_doc AS tipo_documento_nome,
                            cgt.quantidade AS quantidade_clausulas,
                            cgt.quantidade_clausulas_liberadas AS quantidade_clausulas_liberadas,
                            dct.data_liberacao_clausulas AS data_liberacao_clausulas
                        from doc_sind dct
                        join tipo_doc td on dct.tipo_doc_idtipo_doc = td.idtipo_doc
                        join lateral (
                            select count(1) AS quantidade, sum((case when (cgt.liberado = 'S') then 1 else 0 end)) AS quantidade_clausulas_liberadas
                            from clausula_geral cgt
                            where cgt.doc_sind_id_documento = dct.id_doc
                            and exists(
                                select 1 from clausula_geral_estrutura_clausula cgec
                                where cgt.id_clau = cgec.clausula_geral_id_clau
                                and cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional <> 170
                            )
                            and cgt.consta_no_documento = 1
                        ) cgt on true;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW clausula_geral_info_adicional_vw AS
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
                        sindicatos_patronais.denominacoes AS denominacoes_patronais,
                        dsindt.sind_laboral AS documento_sindicatos_laborais,
                        sindicatos_laborais.denominacoes AS denominacoes_laborais,
                        dsindt.cnae_doc AS documento_atividades_economicas,
                        dsindt.uf AS documento_uf,
                        dsindt.validade_inicial AS documento_validade_inicial,
                        dsindt.validade_final AS documento_validade_final,
                        dsindt.database_doc AS documento_database,
                        ect.tipo_clausula AS estrutura_clausula_tipo,
                        tiat.idtipodado AS informacao_adicional_tipo_dado,
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
                        cgect.estrutura_clausula_id_estruturaclausula AS estrutura_clausula_id,
                        dsindt.cliente_estabelecimento AS estabelecimentos_json,
                        dsindt.sind_laboral AS sind_laboral,
                        dsindt.sind_patronal AS sind_patronal
                    from clausula_geral cgt
                    join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau
                    join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
                    join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                    join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = tiat.cdtipoinformacaoadicional
                    join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                    join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                    join lateral (
                        select group_concat(sp.denominacao_sp separator '; ') AS denominacoes
                        from sind_patr sp
                        join documento_sindicato_patronal_tb dspt on dspt.documento_id = cgt.doc_sind_id_documento
                        where sp.id_sindp = dspt.sindicato_patronal_id
                    ) sindicatos_patronais
                    join lateral (
                        select group_concat(sinde.denominacao_sinde separator '; ') AS denominacoes
                        from sind_emp sinde
                        join documento_sindicato_laboral_tb dslt on dslt.documento_id = cgt.doc_sind_id_documento
                        where sinde.id_sinde = dslt.sindicato_laboral_id
                    ) sindicatos_laborais
                    where cgt.consta_no_documento = 1;");

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
                from clausula_geral cgt
                join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau
                join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
                join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = tiat.cdtipoinformacaoadicional
                join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                left join lateral (
                    select
                        iagt.id_grupo AS id_grupo,
                        iagt.ad_tipoinformacaoadicional_id AS ad_tipoinformacaoadicional_id,
                        iagt.informacaoadicional_no_grupo AS informacaoadicional_no_grupo,
                        iagt.sequencia AS sequencia,
                        iagt.exibe_comparativo_mapa_sindical AS exibe_comparativo_mapa_sindical
                    from estrutura_clausulas_ad_tipoinformacaoadicional ecat
                    join informacao_adicional_grupo iagt on ecat.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.ad_tipoinformacaoadicional_id
                    where cgect.nome_informacao = ecat.estrutura_clausula_id_estruturaclausula
                    and cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.informacaoadicional_no_grupo
                    limit 1
                ) iagt on true
                where cgt.consta_no_documento = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clausula_cliente_tb");

            migrationBuilder.Sql(@"drop procedure obter_clausulas_ultimo_ano_sindicatos_laborais;");

            migrationBuilder.Sql(@"CREATE PROCEDURE obter_clausulas_ultimo_ano_sindicatos_laborais(in usuario_id INT)
                                BEGIN

                                       DECLARE query VARCHAR(1000);
  
	                                   DELETE FROM documento_sindicato_mais_recente_usuario_tb dsmrut WHERE dsmrut.usuario_id = usuario_id;
  
	
                                        CREATE TABLE IF NOT EXISTS documento_sindicato_temp (
                                            documento_sindical_id INT,
                                            sindicato_laboral_id INT,
                                            tipo_documento_id INT,
                                            nome_doc VARCHAR(255),
                                            mes_validade_inicial INT,
                                            ano_validade_inicial INT,
                                            mes_validade_final INT,
                                            ano_validade_final INT,
                                            database_doc VARCHAR(255),
                                            validade_inicial DATE,
                                            validade_final DATE,
                                            sigla VARCHAR(500),
                                            ano_mes_validade_inicial INT,
                                            ano_mes_validade_final INT,
                                            usuario_id int
                                        );
   
                                      CREATE TABLE IF NOT EXISTS documento_sindicato_mais_recente_temp (
                                            sindicato_laboral_id INT,
                                            ano_mes_validade_inicial INT,
                                            ano_mes_validade_final INT,
                                            documento_sindical_id INT,
                                            row_num INT,
                                            usuario_id int
                                        );
           
                                       
                                    CREATE TABLE IF NOT EXISTS documento_sindicato_clausula_temp (
                                        documento_sindical_id INT,
                                        sindicato_laboral_id INT,
                                        tipo_documento_id INT,
                                        nome_doc VARCHAR(255),
                                        mes_validade_inicial INT,
                                        ano_validade_inicial INT,
                                        mes_validade_final INT,
                                        ano_validade_final INT,
                                        database_doc VARCHAR(255),
                                        validade_inicial DATE,
                                        validade_final DATE,
                                        sigla VARCHAR(500),
                                        ano_mes_validade_inicial INT,
                                        ano_mes_validade_final INT,
                                        clausula_geral_id int,
                                        estrutura_clausula_geral_id int,
                                        usuario_id int
                                    );
                                   
                                   DELETE FROM documento_sindicato_temp where usuario_id = usuario_id;
                                   DELETE FROM documento_sindicato_mais_recente_temp where usuario_id = usuario_id;
	                               DELETE FROM documento_sindicato_clausula_temp where usuario_id = usuario_id;
  
	
                                    INSERT INTO documento_sindicato_temp (
                                    documento_sindical_id,
                                    sindicato_laboral_id,
                                    tipo_documento_id,
                                    nome_doc,
                                    mes_validade_inicial,
                                    ano_validade_inicial,
                                    mes_validade_final,
                                    ano_validade_final,
                                    database_doc,
                                    validade_inicial,
                                    validade_final,
                                    sigla,
                                    ano_mes_validade_inicial,
                                    ano_mes_validade_final,
                                    usuario_id
                                    )
                                    select dct.id_doc documento_sindical_id, sempt.id sindicato_laboral_id, dct.tipo_doc_idtipo_doc  tipo_documento_id, tdt.nome_doc, month(dct.validade_inicial) mes_validade_inicial, year(dct.validade_inicial) ano_validade_inicial, month(dct.validade_final) mes_validade_final, year(dct.validade_final) ano_validade_final, 
                                    dct.database_doc, dct.validade_inicial, dct.validade_final, sempt.sigla, year(dct.validade_inicial) *100 + month(dct.validade_inicial) ano_mes_validade_inicial, year(dct.validade_final) * 100 + month(dct.validade_final) ano_mes_validade_final, usuario_id
                                    from doc_sind dct
                                    inner join tipo_doc tdt on tdt.idtipo_doc = dct.tipo_doc_idtipo_doc,
                                    json_table(dct.sind_laboral, '$[*]' columns (
    id int4 path '$.id',
    sigla varchar(500) path '$.sigla',
    codigo varchar(500) path '$.codigo'
                                    )) sempt
                                    where dct.data_liberacao_clausulas is not null
                                   and YEAR(dct.data_liberacao_clausulas) > 1900;

                                    INSERT INTO documento_sindicato_mais_recente_temp (
                                    sindicato_laboral_id,
                                    ano_mes_validade_inicial,
                                    ano_mes_validade_final,
                                    documento_sindical_id,
                                    row_num,
                                    usuario_id
                                    )
                                    select x.*, usuario_id from (
                                            WITH RankedRows AS (
                                                  SELECT
                                                    sindicato_laboral_id,
                                                    ano_mes_validade_inicial,
                                                    ano_mes_validade_final, 
                                                    documento_sindical_id,
                                                    ROW_NUMBER() OVER (PARTITION BY sindicato_laboral_id ORDER BY ano_mes_validade_inicial DESC, ano_mes_validade_final desc) AS row_num
                                                  FROM documento_sindicato_temp
                                                  WHERE tipo_documento_id IN (5, 6)
                                                )
                                                SELECT
                                                  *
                                                FROM RankedRows
                                                WHERE row_num = 1) x;
               
               
                                    insert into documento_sindicato_mais_recente_usuario_tb
    	                                (sindicato_laboral_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, clausula_geral_id, estrutura_clausula_id, usuario_id, row_num)
   	                                select dstt.sindicato_laboral_id, dstt.ano_mes_validade_inicial, dstt.ano_mes_validade_final, dstt.documento_sindical_id, cgt.clausula_id, cgt.estrutura_clausula_id, usuario_id, row_num                                   	
                                    from documento_sindicato_mais_recente_temp dstt
                                    inner join clausulas_vw cgt on dstt.documento_sindical_id = cgt.documento_id
                                    where dstt.row_num = 1
   
                                   insert into documento_sindicato_mais_recente_usuario_tb
    	                                (sindicato_laboral_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, clausula_geral_id, estrutura_clausula_id, usuario_id, row_num)
    	                              SELECT 
									    sindicato_laboral_id,
									    ano_mes_validade_inicial,
									    ano_mes_validade_final,
									    documento_sindical_id,
									    clausula_id,
									    estrutura_clausula_id,
									    usuario_id,
									    row_num
									FROM (
									    SELECT 
									        dstt.sindicato_laboral_id,
									        dstt.ano_mes_validade_inicial,
									        dstt.ano_mes_validade_final,
									        dstt.documento_sindical_id,
									        cgt.clausula_id,
									        cgt.estrutura_clausula_id,
									        usuario_id,
									        ROW_NUMBER() OVER (PARTITION BY cgt.estrutura_clausula_id ORDER BY dstt.sindicato_laboral_id) AS row_num
									    FROM documento_sindicato_temp dstt
									    INNER JOIN documento_sindicato_mais_recente_temp dsmrtt 
									        ON dstt.sindicato_laboral_id = dsmrtt.sindicato_laboral_id
									        AND dstt.ano_mes_validade_inicial BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
									        AND dstt.ano_mes_validade_final BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
									        AND dstt.documento_sindical_id <> dsmrtt.documento_sindical_id
									    INNER JOIN LATERAL (
									        SELECT 
									            cgt.estrutura_clausula_id,
									            MAX(cgt.clausula_id) clausula_id 
									        FROM clausulas_vw cgt 
									        WHERE dstt.documento_sindical_id = cgt.documento_id
									        GROUP BY 1
									    ) cgt ON TRUE
									    WHERE NOT EXISTS (
									        SELECT 1 
									        FROM documento_sindicato_mais_recente_usuario_tb dsct 
									        WHERE dsct.estrutura_clausula_id = cgt.estrutura_clausula_id
									        AND dsct.sindicato_laboral_id = dstt.sindicato_laboral_id
									    )
									) AS subquery
									WHERE row_num = 1;

                                END;");


            migrationBuilder.Sql(@"DROP VIEW IF EXISTS documento_mapa_sindical_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW documento_mapa_sindical_vw AS
                        select
                            dct.id_doc AS documento_id,
                            dct.sind_patronal AS documento_sindicato_patronal,
                            dct.sind_laboral AS documento_sindicato_laboral,
                            dct.data_reg_mte AS data_registro,
                            dct.data_aprovacao AS documento_data_aprovacao,
                            dct.database_doc AS documento_database,
                            dct.tipo_doc_idtipo_doc AS documento_tipo_id,
                            dct.cnae_doc AS documento_cnae,
                            dct.abrangencia AS documento_abrangencia,
                            dct.cliente_estabelecimento AS documento_estabelecimento,
                            dct.validade_inicial AS documento_validade_inicial,
                            dct.validade_final AS documento_validade_final,
                            dct.uf AS documento_uf,
                            dct.descricao_documento AS documento_descricao,
                            dct.titulo_documento AS documento_titulo,
                            td.nome_doc AS tipo_documento_nome,
                            cgt.quantidade AS quantidade_clausulas,
                            cgt.quantidade_clausulas_liberadas AS quantidade_clausulas_liberadas,
                            dct.data_liberacao_clausulas AS data_liberacao_clausulas
                        from doc_sind dct
                        join tipo_doc td on dct.tipo_doc_idtipo_doc = td.idtipo_doc
                        join lateral (
                            select count(1) AS quantidade, sum((case when (cgt.liberado = 'S') then 1 else 0 end)) AS quantidade_clausulas_liberadas
                            from clausula_geral cgt
                            where cgt.doc_sind_id_documento = dct.id_doc
                            and exists(
                                select 1 from clausula_geral_estrutura_clausula cgec
                                where cgt.id_clau = cgec.clausula_geral_id_clau
                                and cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional <> 170
                            )
                        ) cgt on true;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW clausula_geral_info_adicional_vw AS
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
                        sindicatos_patronais.denominacoes AS denominacoes_patronais,
                        dsindt.sind_laboral AS documento_sindicatos_laborais,
                        sindicatos_laborais.denominacoes AS denominacoes_laborais,
                        dsindt.cnae_doc AS documento_atividades_economicas,
                        dsindt.uf AS documento_uf,
                        dsindt.validade_inicial AS documento_validade_inicial,
                        dsindt.validade_final AS documento_validade_final,
                        dsindt.database_doc AS documento_database,
                        ect.tipo_clausula AS estrutura_clausula_tipo,
                        tiat.idtipodado AS informacao_adicional_tipo_dado,
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
                        cgect.estrutura_clausula_id_estruturaclausula AS estrutura_clausula_id,
                        dsindt.cliente_estabelecimento AS estabelecimentos_json,
                        dsindt.sind_laboral AS sind_laboral,
                        dsindt.sind_patronal AS sind_patronal
                    from clausula_geral cgt
                    join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau
                    join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
                    join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                    join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = tiat.cdtipoinformacaoadicional
                    join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                    join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                    join lateral (
                        select group_concat(sp.denominacao_sp separator '; ') AS denominacoes
                        from sind_patr sp
                        join documento_sindicato_patronal_tb dspt on dspt.documento_id = cgt.doc_sind_id_documento
                        where sp.id_sindp = dspt.sindicato_patronal_id
                    ) sindicatos_patronais
                    join lateral (
                        select group_concat(sinde.denominacao_sinde separator '; ') AS denominacoes
                        from sind_emp sinde
                        join documento_sindicato_laboral_tb dslt on dslt.documento_id = cgt.doc_sind_id_documento
                        where sinde.id_sinde = dslt.sindicato_laboral_id
                    ) sindicatos_laborais;");

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
                from clausula_geral cgt
                join clausula_geral_estrutura_clausula cgect on cgt.id_clau = cgect.clausula_geral_id_clau
                join estrutura_clausula ect on cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
                join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                join ad_tipoinformacaoadicional tiat on cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = tiat.cdtipoinformacaoadicional
                join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                left join lateral (
                    select
                        iagt.id_grupo AS id_grupo,
                        iagt.ad_tipoinformacaoadicional_id AS ad_tipoinformacaoadicional_id,
                        iagt.informacaoadicional_no_grupo AS informacaoadicional_no_grupo,
                        iagt.sequencia AS sequencia,
                        iagt.exibe_comparativo_mapa_sindical AS exibe_comparativo_mapa_sindical
                    from estrutura_clausulas_ad_tipoinformacaoadicional ecat
                    join informacao_adicional_grupo iagt on ecat.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.ad_tipoinformacaoadicional_id
                    where cgect.nome_informacao = ecat.estrutura_clausula_id_estruturaclausula
                    and cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.informacaoadicional_no_grupo
                    limit 1
                ) iagt on true;");
        }
    }
}
