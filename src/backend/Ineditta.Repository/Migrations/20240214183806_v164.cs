using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v164 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter table clausula_geral_estrutura_clausula
                    drop foreign key fk_clausula_geral_estrutura_clausula_x_info_grupo;");

            migrationBuilder.Sql(@"ALTER TABLE clausula_geral_estrutura_clausula
                    ADD CONSTRAINT fk_clausula_geral_estrutura_clausula_x_info_grupo
                    FOREIGN KEY (id_info_tipo_grupo)
                    REFERENCES ad_tipoinformacaoadicional(cdtipoinformacaoadicional)
                    ON DELETE CASCADE;");

            migrationBuilder.Sql(@"
                update doc_sind set data_liberacao_clausulas = null
                where data_liberacao_clausulas = '0000-00-00' OR data_liberacao_clausulas = '0001-01-01'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter table clausula_geral_estrutura_clausula
                    drop foreign key fk_clausula_geral_estrutura_clausula_x_info_grupo;");

            migrationBuilder.Sql(@"ALTER TABLE clausula_geral_estrutura_clausula
                    ADD CONSTRAINT fk_clausula_geral_estrutura_clausula_x_informacao_adicional_grup
                    FOREIGN KEY (id_info_tipo_grupo)
                    REFERENCES ad_tipoinformacaoadicional(cdtipoinformacaoadicional)
                    ON DELETE CASCADE;");
        }
    }
}
