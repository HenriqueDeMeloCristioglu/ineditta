using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v137 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS inserir_documento_sindicatos;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE inserir_documento_sindicatos()
                BEGIN
                    INSERT INTO documento_sindicato_patronal_tb (documento_id, sindicato_patronal_id)
                    SELECT 
                        ds.id_doc documento_id,
                        jt.id sindicato_patronal_id
                    FROM 
                        doc_sind ds,
                        JSON_TABLE(ds.sind_patronal, '$[*].id' COLUMNS (id INT PATH '$')) AS jt
                    WHERE jt.id IS NOT NULL;

                    INSERT INTO documento_sindicato_laboral_tb (documento_id, sindicato_laboral_id)
                    SELECT 
                        ds.id_doc documento_id,
                        jt.id sindicato_laboral_id
                    FROM 
                        doc_sind ds,
                        JSON_TABLE(ds.sind_laboral, '$[*].id' COLUMNS (id INT PATH '$')) AS jt
                    WHERE jt.id IS NOT NULL;
                END;
            ");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS obter_documento_ultimo_ano_sindicatos_laborais;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE obter_documento_ultimo_ano_sindicatos_laborais(in usuario_id INT)
                BEGIN

                    DECLARE query VARCHAR(1000);
  
	                DELETE FROM documento_sindicato_mais_recente_usuario_tb dsmrut WHERE dsmrut.usuario_id = usuario_id;
	
                    CREATE TABLE IF NOT EXISTS documento_sindicato_temp (
                        documento_sindical_id INT,
                        sindicato_laboral_id INT,
                        sindicato_patronal_id int,
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
                        sindicato_patronal_id INT,
                        ano_mes_validade_inicial INT,
                        ano_mes_validade_final INT,
                        documento_sindical_id INT,
                        row_num INT,
                        usuario_id int
                    );
                                   
                    DELETE FROM documento_sindicato_temp where usuario_id = usuario_id;
                    DELETE FROM documento_sindicato_mais_recente_temp where usuario_id = usuario_id;
	
                    INSERT INTO documento_sindicato_temp (
                        documento_sindical_id,
                        sindicato_laboral_id,
                        sindicato_patronal_id,
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
                    select dct.id_doc documento_sindical_id, sempt.id sindicato_laboral_id, spmpt.id sindicato_patronal_id, dct.tipo_doc_idtipo_doc  tipo_documento_id, tdt.nome_doc, month(dct.validade_inicial) mes_validade_inicial, year(dct.validade_inicial) ano_validade_inicial, month(dct.validade_final) mes_validade_final, year(dct.validade_final) ano_validade_final, 
                    dct.database_doc, dct.validade_inicial, dct.validade_final, sempt.sigla, year(dct.validade_inicial) *100 + month(dct.validade_inicial) ano_mes_validade_inicial, year(dct.validade_final) * 100 + month(dct.validade_final) ano_mes_validade_final, usuario_id
                    from doc_sind dct
                    inner join tipo_doc tdt on tdt.idtipo_doc = dct.tipo_doc_idtipo_doc
                    left join lateral (
                        SELECT 
                            se.id_sinde id, 
                            se.sigla_sinde sigla, 
                            dsltb.documento_id documento_id
                        FROM documento_sindicato_laboral_tb dsltb
                        JOIN sind_emp se ON dsltb.sindicato_laboral_id = se.id_sinde
                    ) sempt ON sempt.documento_id = dct.id_doc
                    left join lateral (
                        SELECT 
                            sp.id_sindp id, 
                            sp.sigla_sp sigla, 
                            dsptb.documento_id documento_id
                        FROM documento_sindicato_patronal_tb dsptb
                        JOIN sind_patr sp ON dsptb.sindicato_patronal_id = sp.id_sindp
                    ) spmpt ON spmpt.documento_id = dct.id_doc
                    where dct.data_aprovacao is not null
                    and YEAR(dct.data_aprovacao) > 1900;

                    INSERT INTO documento_sindicato_mais_recente_temp (
                        sindicato_laboral_id,
                        sindicato_patronal_id,
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
                                sindicato_patronal_id,
                                ano_mes_validade_inicial,
                                ano_mes_validade_final, 
                                documento_sindical_id,
                                ROW_NUMBER() OVER (PARTITION BY sindicato_laboral_id, sindicato_patronal_id ORDER BY ano_mes_validade_inicial DESC, ano_mes_validade_final desc) AS row_num
                                FROM documento_sindicato_temp
                                WHERE tipo_documento_id IN (5, 6)
                                and documento_sindicato_temp.usuario_id  = usuario_id
                            )
                        SELECT
                            *
                        FROM RankedRows
                        WHERE row_num = 1) x;
               
               
                    insert into documento_sindicato_mais_recente_usuario_tb
    	                (sindicato_laboral_id, sindicato_patronal_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, usuario_id, row_num)
   	                select dstt.sindicato_laboral_id, dstt.sindicato_patronal_id, dstt.ano_mes_validade_inicial, dstt.ano_mes_validade_final, dstt.documento_sindical_id, usuario_id, row_num                                   	
                    from documento_sindicato_mais_recente_temp dstt
                    where dstt.row_num = 1
                          and dstt.usuario_id = usuario_id
                          and sindicato_laboral_id IS NOT NULL;
   
                    insert into documento_sindicato_mais_recente_usuario_tb
    	                (sindicato_laboral_id, sindicato_patronal_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, usuario_id, row_num)
    	                SELECT 
					    sindicato_laboral_id,
					    sindicato_patronal_id,
					    ano_mes_validade_inicial,
					    ano_mes_validade_final,
					    documento_sindical_id,
					    usuario_id,
					    row_num
				    FROM (
					    SELECT 
						    dstt.sindicato_laboral_id,
						    dstt.sindicato_patronal_id,
						    dstt.ano_mes_validade_inicial,
						    dstt.ano_mes_validade_final,
						    dstt.documento_sindical_id,
						    usuario_id,
						    ROW_NUMBER() OVER (PARTITION BY dstt.documento_sindical_id ORDER BY dstt.sindicato_laboral_id) AS row_num
					    FROM documento_sindicato_temp dstt
					    INNER JOIN documento_sindicato_mais_recente_temp dsmrtt 
						    ON dstt.sindicato_laboral_id = dsmrtt.sindicato_laboral_id
						    AND dstt.sindicato_patronal_id = dsmrtt.sindicato_patronal_id
						    AND dstt.ano_mes_validade_inicial BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
						    AND dstt.ano_mes_validade_final BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
						    AND dstt.documento_sindical_id <> dsmrtt.documento_sindical_id
						    and dstt.usuario_id = dsmrtt.usuario_id
						    and dstt.usuario_id = usuario_id
					    WHERE NOT EXISTS (
						    SELECT 1 
						    FROM documento_sindicato_mais_recente_usuario_tb dsct 
						    WHERE TRUE
						    AND dsct.sindicato_laboral_id = dstt.sindicato_laboral_id
						    AND dsct.sindicato_patronal_id = dstt.sindicato_patronal_id
						    and dsct.usuario_id = usuario_id
					    )
                        AND dstt.sindicato_laboral_id IS NOT NULL
				    ) AS subquery
				    WHERE row_num = 1;
								
				    DELETE d1
				    FROM documento_sindicato_mais_recente_usuario_tb d1
				    JOIN (
					    SELECT documento_sindical_id, MIN(id) AS menor_id
					    FROM documento_sindicato_mais_recente_usuario_tb dsmrut
					    WHERE dsmrut.usuario_id = usuario_id
					    GROUP BY documento_sindical_id
					    HAVING COUNT(1) > 1
				    ) x ON d1.documento_sindical_id = x.documento_sindical_id
					    AND d1.id <> x.menor_id
					    AND d1.usuario_id = usuario_id;

                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS inserir_documento_sindicatos;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE inserir_documento_sindicatos()
                BEGIN
                    INSERT INTO documento_sindicato_patronal_tb (documento_id, sindicato_patronal_id)
                    SELECT 
                        ds.id_doc documento_id,
                        jt.id sindicato_patronal_id
                    FROM 
                        doc_sind ds,
                        JSON_TABLE(ds.sind_patronal, '$[*].id' COLUMNS (id INT PATH '$')) AS jt;

                    INSERT INTO documento_sindicato_laboral_tb (documento_id, sindicato_laboral_id)
                    SELECT 
                        ds.id_doc documento_id,
                        jt.id sindicato_laboral_id
                    FROM 
                        doc_sind ds,
                        JSON_TABLE(ds.sind_laboral, '$[*].id' COLUMNS (id INT PATH '$')) AS jt;
                END;
            ");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS obter_documento_ultimo_ano_sindicatos_laborais;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE obter_documento_ultimo_ano_sindicatos_laborais(in usuario_id INT)
                BEGIN

                    DECLARE query VARCHAR(1000);
  
	                DELETE FROM documento_sindicato_mais_recente_usuario_tb dsmrut WHERE dsmrut.usuario_id = usuario_id;
  
	
                    CREATE TABLE IF NOT EXISTS documento_sindicato_temp (
                        documento_sindical_id INT,
                        sindicato_laboral_id INT,
                        sindicato_patronal_id int,
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
                        sindicato_patronal_id INT,
                        ano_mes_validade_inicial INT,
                        ano_mes_validade_final INT,
                        documento_sindical_id INT,
                        row_num INT,
                        usuario_id int
                    );
                                   
                    DELETE FROM documento_sindicato_temp where usuario_id = usuario_id;
                    DELETE FROM documento_sindicato_mais_recente_temp where usuario_id = usuario_id;
	
                    INSERT INTO documento_sindicato_temp (
                        documento_sindical_id,
                        sindicato_laboral_id,
                        sindicato_patronal_id,
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
                    select dct.id_doc documento_sindical_id, sempt.id sindicato_laboral_id, spmpt.id sindicato_patronal_id, dct.tipo_doc_idtipo_doc  tipo_documento_id, tdt.nome_doc, month(dct.validade_inicial) mes_validade_inicial, year(dct.validade_inicial) ano_validade_inicial, month(dct.validade_final) mes_validade_final, year(dct.validade_final) ano_validade_final, 
                    dct.database_doc, dct.validade_inicial, dct.validade_final, sempt.sigla, year(dct.validade_inicial) *100 + month(dct.validade_inicial) ano_mes_validade_inicial, year(dct.validade_final) * 100 + month(dct.validade_final) ano_mes_validade_final, usuario_id
                    from doc_sind dct
                    inner join tipo_doc tdt on tdt.idtipo_doc = dct.tipo_doc_idtipo_doc
                    left join lateral (
                        SELECT 
                            se.id_sinde id, 
                            se.sigla_sinde sigla, 
                            dsltb.documento_id documento_id
                        FROM documento_sindicato_laboral_tb dsltb
                        JOIN sind_emp se ON dsltb.sindicato_laboral_id = se.id_sinde
                    ) sempt ON sempt.documento_id = dct.id_doc
                    left join lateral (
                        SELECT 
                            sp.id_sindp id, 
                            sp.sigla_sp sigla, 
                            dsptb.documento_id documento_id
                        FROM documento_sindicato_patronal_tb dsptb
                        JOIN sind_patr sp ON dsptb.sindicato_patronal_id = sp.id_sindp
                    ) spmpt ON spmpt.documento_id = dct.id_doc
                    where dct.data_aprovacao is not null
                    and YEAR(dct.data_aprovacao) > 1900;

                    INSERT INTO documento_sindicato_mais_recente_temp (
                        sindicato_laboral_id,
                        sindicato_patronal_id,
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
                                sindicato_patronal_id,
                                ano_mes_validade_inicial,
                                ano_mes_validade_final, 
                                documento_sindical_id,
                                ROW_NUMBER() OVER (PARTITION BY sindicato_laboral_id, sindicato_patronal_id ORDER BY ano_mes_validade_inicial DESC, ano_mes_validade_final desc) AS row_num
                                FROM documento_sindicato_temp
                                WHERE tipo_documento_id IN (5, 6)
                                and documento_sindicato_temp.usuario_id  = usuario_id
                            )
                        SELECT
                            *
                        FROM RankedRows
                        WHERE row_num = 1) x;
               
               
                    insert into documento_sindicato_mais_recente_usuario_tb
    	                (sindicato_laboral_id, sindicato_patronal_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, usuario_id, row_num)
   	                select dstt.sindicato_laboral_id, dstt.sindicato_patronal_id, dstt.ano_mes_validade_inicial, dstt.ano_mes_validade_final, dstt.documento_sindical_id, usuario_id, row_num                                   	
                    from documento_sindicato_mais_recente_temp dstt
                    where dstt.row_num = 1
                          and dstt.usuario_id = usuario_id;
   
                    insert into documento_sindicato_mais_recente_usuario_tb
    	                (sindicato_laboral_id, sindicato_patronal_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, usuario_id, row_num)
    	                SELECT 
					    sindicato_laboral_id,
					    sindicato_patronal_id,
					    ano_mes_validade_inicial,
					    ano_mes_validade_final,
					    documento_sindical_id,
					    usuario_id,
					    row_num
				    FROM (
					    SELECT 
						    dstt.sindicato_laboral_id,
						    dstt.sindicato_patronal_id,
						    dstt.ano_mes_validade_inicial,
						    dstt.ano_mes_validade_final,
						    dstt.documento_sindical_id,
						    usuario_id,
						    ROW_NUMBER() OVER (PARTITION BY dstt.documento_sindical_id ORDER BY dstt.sindicato_laboral_id) AS row_num
					    FROM documento_sindicato_temp dstt
					    INNER JOIN documento_sindicato_mais_recente_temp dsmrtt 
						    ON dstt.sindicato_laboral_id = dsmrtt.sindicato_laboral_id
						    AND dstt.sindicato_patronal_id = dsmrtt.sindicato_patronal_id
						    AND dstt.ano_mes_validade_inicial BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
						    AND dstt.ano_mes_validade_final BETWEEN dsmrtt.ano_mes_validade_inicial AND dsmrtt.ano_mes_validade_final
						    AND dstt.documento_sindical_id <> dsmrtt.documento_sindical_id
						    and dstt.usuario_id = dsmrtt.usuario_id
						    and dstt.usuario_id = usuario_id
					    WHERE NOT EXISTS (
						    SELECT 1 
						    FROM documento_sindicato_mais_recente_usuario_tb dsct 
						    WHERE TRUE
						    AND dsct.sindicato_laboral_id = dstt.sindicato_laboral_id
						    AND dsct.sindicato_patronal_id = dstt.sindicato_patronal_id
						    and dsct.usuario_id = usuario_id
					    )
				    ) AS subquery
				    WHERE row_num = 1;
								
				    DELETE d1
				    FROM documento_sindicato_mais_recente_usuario_tb d1
				    JOIN (
					    SELECT documento_sindical_id, MIN(id) AS menor_id
					    FROM documento_sindicato_mais_recente_usuario_tb dsmrut
					    WHERE dsmrut.usuario_id = usuario_id
					    GROUP BY documento_sindical_id
					    HAVING COUNT(1) > 1
				    ) x ON d1.documento_sindical_id = x.documento_sindical_id
					    AND d1.id <> x.menor_id
					    AND d1.usuario_id = usuario_id;

                END;
            ");
        }
    }
}
