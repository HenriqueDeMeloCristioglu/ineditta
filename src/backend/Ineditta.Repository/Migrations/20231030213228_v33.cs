using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE
                                    ALGORITHM = UNDEFINED VIEW comparatvio_mapa_sindical_principal_vw AS
                                    select
                                        ngt.id_doc AS negociacao_id,
                                        (
                                        select
                                            group_concat(at_tb.subclasse separator ', ') AS concatenated_subclasses
                                        from
                                            json_table(ngt.cnae_doc, '$[*]' columns (subclasse varchar(255) character set utf8mb4 path '$.subclasse')) at_tb) AS atividade_economica,
                                        (
                                        select
                                            group_concat(ab_tb.municipio separator ', ') AS concatenated_subclasses
                                        from
                                            json_table(ngt.abrangencia, '$[*]' columns (municipio varchar(255) character set utf8mb4 path '$.Municipio')) ab_tb) AS municipios,
                                        (case
                                            when (json_length(ngt.cliente_estabelecimento) > 0) then json_length(ngt.cliente_estabelecimento)
                                            else unt.quantidade
                                        end) AS quantidade_estabelecimentos,
                                        ngt.database_doc AS data_base,
                                        concat(date_format(ngt.validade_inicial, '%d/%m/%Y'), ' até ', date_format(ngt.validade_final, '%d/%m/%Y')) AS vigencia,
                                        ngt.titulo_documento AS nome,
                                        0.01 AS inpc
                                    from
                                        (doc_sind ngt
                                    left join lateral (
                                        select
                                            count(distinct cut.id_unidade) AS quantidade
                                        from
                                            ((cliente_unidades cut
                                        join json_table(cut.cnae_unidade, '$[*].id' columns (id int path '$')) cutcnt on
                                            (true))
                                        join json_table(ngt.cnae_doc, '$[*].id' columns (id int path '$')) ngtcdt on
                                            (true))
                                        where
                                            (cutcnt.id = ngtcdt.id)) unt on
                                        (true));");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE
                                    ALGORITHM = UNDEFINED VIEW comparatvio_mapa_sindical_principal_vw AS
                                    select
                                        ngt.id_doc AS negociacao_id,
                                        (
                                        select
                                            group_concat(at_tb.subclasse separator ', ') AS concatenated_subclasses
                                        from
                                            json_table(ngt.cnae_doc, '$[*]' columns (subclasse varchar(255) character set utf8mb4 path '$.subclasse')) at_tb) AS atividade_economica,
                                        (
                                        select
                                            group_concat(ab_tb.municipio separator ', ') AS concatenated_subclasses
                                        from
                                            json_table(ngt.abrangencia, '$[*]' columns (municipio varchar(255) character set utf8mb4 path '$.Municipio')) ab_tb) AS municipios,
                                        (case
                                            when (json_length(ngt.cliente_estabelecimento) > 0) then json_length(ngt.cliente_estabelecimento)
                                            else unt.quantidade
                                        end) AS quantidade_estabelecimentos,
                                        ngt.database_doc AS data_base,
                                        concat(date_format(ngt.validade_inicial, '%d/%m/%Y'), ' até ', date_format(ngt.validade_final, '%d/%m/%Y')) AS vigencia,
                                        ngt.titulo_documento AS nome,
                                        0.01 AS inpc
                                    from
                                        (doc_sind ngt
                                    left join lateral (
                                        select
                                            count(distinct cut.id_unidade) AS quantidade
                                        from
                                            ((cliente_unidades cut
                                        join json_table(cut.cnae_unidade, '$[*].id' columns (id int path '$')) cutcnt on
                                            (true))
                                        join json_table(ngt.cnae_doc, '$[*].id' columns (id int path '$')) ngtcdt on
                                            (true))
                                        where
                                            (cutcnt.id = ngtcdt.id)) unt on
                                        (true));");

        }
    }
}
