using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documento_mapa_sindical_vw;");
            
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
                                        cgt.quantidade_clausulas_liberadas AS quantidade_clausulas_liberadas
                                    from
                                        ((doc_sind dct
                                    join tipo_doc td on
                                        ((dct.tipo_doc_idtipo_doc = td.idtipo_doc)))
                                    join lateral (
                                        select
                                            count(1) AS quantidade,
                                            count((case when (cgt.liberado = 'S') then 1 else 0 end)) AS quantidade_clausulas_liberadas
                                        from
                                            clausula_geral cgt
                                        where
                                            ((cgt.doc_sind_id_documento = dct.id_doc)
                                                and exists(
                                                select
                                                    1
                                                from
                                                    clausula_geral_estrutura_clausula cgec
                                                where
                                                    ((cgt.id_clau = cgec.clausula_geral_id_clau)
                                                            and (cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional <> 170))))) cgt on
                                        (true));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documento_mapa_sindical_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                                    VIEW documento_mapa_sindical_vw AS
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
                                        td.nome_doc as tipo_documento_nome,
                                        cgt.quantidade quantidade_clausulas,
                                        cgt.quantidade_clausulas_liberadas
                                    FROM 
                                            doc_sind dct
                                    inner join tipo_doc td on dct.tipo_doc_idtipo_doc = td.idtipo_doc
                                    inner join lateral(select count(1) quantidade, count(case when cgt.liberado = 'S' then 1 else 0 end) quantidade_clausulas_liberadas
                                    from clausula_geral cgt 
                                    where cgt.doc_sind_id_documento = dct.id_doc
                                    and exists(select 1 from clausula_geral_estrutura_clausula cgec where cgt.id_clau = cgec.clausula_geral_id_clau and cgt.estrutura_id_estruturaclausula = cgec.estrutura_clausula_id_estruturaclausula
                                    and cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional <> 170)) cgt on true;");
        }
    }
}
