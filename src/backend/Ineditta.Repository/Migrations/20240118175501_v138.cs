using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v138 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE sind_patr sp SET codigo_sp = '' WHERE codigo_sp IS NULL;

                UPDATE sind_patr sp SET cnpj_sp = '00000000000000' WHERE cnpj_sp IS NULL;

                UPDATE sind_patr sp SET sigla_sp = '' WHERE sigla_sp IS NULL;

                UPDATE sind_patr sp SET municipio_sp = '' WHERE municipio_sp IS NULL;

                UPDATE sind_patr sp SET uf_sp = '' WHERE uf_sp IS NULL;

                UPDATE sind_patr sp SET fone1_sp = '000000000' WHERE fone1_sp IS NULL;

                UPDATE sind_patr sp SET razaosocial_sp = '' WHERE razaosocial_sp IS NULL;

                UPDATE sind_patr sp SET denominacao_sp = '' WHERE denominacao_sp IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE sind_patr sp SET codigo_sp = NULL WHERE codigo_sp = '';

                UPDATE sind_patr sp SET cnpj_sp = NULL WHERE cnpj_sp = '';

                UPDATE sind_patr sp SET sigla_sp = NULL WHERE sigla_sp = '';

                UPDATE sind_patr sp SET municipio_sp = NULL WHERE municipio_sp = '';

                UPDATE sind_patr sp SET uf_sp = NULL WHERE uf_sp = '';

                UPDATE sind_patr sp SET fone1_sp = NULL WHERE fone1_sp = '';

                UPDATE sind_patr sp SET razaosocial_sp = NULL WHERE razaosocial_sp = '';

                UPDATE sind_patr sp SET denominacao_sp = NULL WHERE denominacao_sp = '';
            ");
        }
    }
}
