using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v115 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS obter_clausulas_ultimo_ano_sindicatos_laborais;
                    
                                CREATE PROCEDURE obter_clausulas_ultimo_ano_sindicatos_laborais (in usuario_id INT)
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
                                    where dstt.row_num = 1;
   
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS obter_clausulas_ultimo_ano_sindicatos_laborais;
                    
                                CREATE PROCEDURE obter_clausulas_ultimo_ano_sindicatos_laborais(in usuario_id INT)
                                    BEGIN

                                       DECLARE query VARCHAR(1000);
  
	                                   DELETE FROM documento_sindicato_mais_recente_usuario_tb dsmrut WHERE dsmrut.usuario_id = usuario_id;
  
  	                                   DROP TEMPORARY TABLE IF EXISTS documento_sindicato_temp;
	                                   DROP TEMPORARY TABLE IF EXISTS documento_sindicato_mais_recente_temp;
	                                   DROP TABLE IF EXISTS documento_sindicato_clausula_temp;
	
                                        CREATE TEMPORARY TABLE IF NOT EXISTS documento_sindicato_temp (
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
                                            ano_mes_validade_final INT
                                        );
   
                                      CREATE TEMPORARY TABLE IF NOT EXISTS documento_sindicato_mais_recente_temp (
                                            sindicato_laboral_id INT,
                                            ano_mes_validade_inicial INT,
                                            ano_mes_validade_final INT,
                                            documento_sindical_id INT,
                                            row_num INT
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
                                    ano_mes_validade_final
                                    )
                                    select dct.id_doc documento_sindical_id, sempt.id sindicato_laboral_id, dct.tipo_doc_idtipo_doc  tipo_documento_id, tdt.nome_doc, month(dct.validade_inicial) mes_validade_inicial, year(dct.validade_inicial) ano_validade_inicial, month(dct.validade_final) mes_validade_final, year(dct.validade_final) ano_validade_final, 
                                    dct.database_doc, dct.validade_inicial, dct.validade_final, sempt.sigla, year(dct.validade_inicial) *100 + month(dct.validade_inicial) ano_mes_validade_inicial, year(dct.validade_final) * 100 + month(dct.validade_final) ano_mes_validade_final
                                    from doc_sind dct
                                    inner join tipo_doc tdt on tdt.idtipo_doc = dct.tipo_doc_idtipo_doc,
                                    json_table(dct.sind_laboral, '$[*]' columns (
                                        id int4 path '$.id',
                                        sigla varchar(500) path '$.sigla',
                                        codigo varchar(500) path '$.codigo'
                                    )) sempt
                                    where dct.data_liberacao_clausulas is not null;

                                    INSERT INTO documento_sindicato_mais_recente_temp (
                                    sindicato_laboral_id,
                                    ano_mes_validade_inicial,
                                    ano_mes_validade_final,
                                    documento_sindical_id,
                                    row_num
                                    )
                                    select * from (
                                            WITH RankedRows AS (
                                                  SELECT
                                                    sindicato_laboral_id,
                                                    ano_mes_validade_inicial,
                                                    ano_mes_validade_final, 
                                                    documento_sindical_id,
                                                    ROW_NUMBER() OVER (PARTITION BY sindicato_laboral_id ORDER BY ano_mes_validade_inicial DESC) AS row_num
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
                                    where dstt.row_num = 1;
   
                                   insert into documento_sindicato_mais_recente_usuario_tb
    	                                (sindicato_laboral_id, ano_mes_validade_inicial, ano_mes_validade_final, documento_sindical_id, clausula_geral_id, estrutura_clausula_id, usuario_id, row_num)
   		                                 select dstt.sindicato_laboral_id, dstt.ano_mes_validade_inicial, dstt.ano_mes_validade_final, dstt.documento_sindical_id, cgt.clausula_id, cgt.estrutura_clausula_id, usuario_id, row_num 
                                         from documento_sindicato_temp dstt
                                         inner join documento_sindicato_mais_recente_temp dsmrtt 
                                         on dstt.sindicato_laboral_id = dsmrtt.sindicato_laboral_id
                                         and dstt.ano_mes_validade_inicial between dsmrtt.ano_mes_validade_inicial and dsmrtt.ano_mes_validade_final
                                         and dstt.ano_mes_validade_final between dsmrtt.ano_mes_validade_inicial and dsmrtt.ano_mes_validade_final
                                         and dstt.documento_sindical_id <> dsmrtt.documento_sindical_id
                                         inner join lateral (select cgt.estrutura_clausula_id, max(cgt.clausula_id) clausula_id 
                                                             from clausulas_vw cgt where dstt.documento_sindical_id = cgt.documento_id
                                                             group by 1) cgt on true
                                         where not exists(select 1 from documento_sindicato_mais_recente_usuario_tb dsct 
         					                                where dsct.documento_sindical_id = dstt.documento_sindical_id
         					                                and dsct.estrutura_clausula_id = cgt.estrutura_clausula_id);
   
                                   delete from documento_sindicato_mais_recente_usuario_tb where clausula_geral_id is null;
  
                                   select * from documento_sindicato_mais_recente_usuario_tb;
  
                                   select * from documento_sindicato_mais_recente_usuario_tb where sindicato_laboral_id = 53;
  
                                  select sindicato_laboral_id, count(distinct(documento_sindical_id))
	                                from documento_sindicato_mais_recente_usuario_tb
	                                group by 1
	                                having count(1) > 1;

                                    DROP TEMPORARY TABLE IF EXISTS documento_sindicato_temp;
                                    DROP TEMPORARY TABLE IF EXISTS documento_sindicato_mais_recente_temp;
                                    DROP TABLE IF EXISTS documento_sindicato_clausula_temp;
                                END;");
        }
    }
}
