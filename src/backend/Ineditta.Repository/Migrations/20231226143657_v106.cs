using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v106 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ConvertMonthYear_ENtoPT;");

            migrationBuilder.Sql(@"CREATE FUNCTION ConvertMonthYear_ENtoPT(en_monthYear VARCHAR(10)) RETURNS varchar(10) CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci
                                BEGIN
                                    RETURN CASE 
                                        WHEN UPPER(en_monthYear) LIKE 'JAN/%' THEN REPLACE(en_monthYear, 'Jan', 'JAN')
                                        WHEN UPPER(en_monthYear) LIKE 'FEV/%' THEN REPLACE(en_monthYear, 'Fev', 'FEV')
                                        WHEN UPPER(en_monthYear) LIKE 'MAR/%' THEN REPLACE(en_monthYear, 'Mar', 'MAR')
                                        WHEN UPPER(en_monthYear) LIKE 'ABR/%' THEN REPLACE(en_monthYear, 'Abr', 'ABR')
                                        WHEN UPPER(en_monthYear) LIKE 'MAI/%' THEN REPLACE(en_monthYear, 'Mai', 'MAI')
                                        WHEN UPPER(en_monthYear) LIKE 'JUN/%' THEN REPLACE(en_monthYear, 'Jun', 'JUN')
                                        WHEN UPPER(en_monthYear) LIKE 'JUL/%' THEN REPLACE(en_monthYear, 'Jul', 'JUL')
                                        WHEN UPPER(en_monthYear) LIKE 'AGO/%' THEN REPLACE(en_monthYear, 'Ago', 'AGO')
                                        WHEN UPPER(en_monthYear) LIKE 'SET/%' THEN REPLACE(en_monthYear, 'Set', 'SET')
                                        WHEN UPPER(en_monthYear) LIKE 'OUT/%' THEN REPLACE(en_monthYear, 'Out', 'OUT')
                                        WHEN UPPER(en_monthYear) LIKE 'NOV/%' THEN REPLACE(en_monthYear, 'Nov', 'NOV')
                                        WHEN UPPER(en_monthYear) LIKE 'DEZ/%' THEN REPLACE(en_monthYear, 'Dez', 'DEZ')
                                        ELSE en_monthYear
                                    END;
                                END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ConvertMonthYear_ENtoPT;");

            migrationBuilder.Sql(@"CREATE FUNCTION ConvertMonthYear_ENtoPT(en_monthYear VARCHAR(10)) RETURNS varchar(10) CHARSET utf8mb4
                                BEGIN
                                    RETURN CASE 
                                        WHEN en_monthYear LIKE 'Jan/%' THEN REPLACE(en_monthYear, UPPER('Jan'), 'JAN')
                                        WHEN en_monthYear LIKE 'Feb/%' THEN REPLACE(en_monthYear, UPPER('Feb'), 'FEV')
                                        WHEN en_monthYear LIKE 'Mar/%' THEN REPLACE(en_monthYear, UPPER('Mar'), 'MAR')
                                        WHEN en_monthYear LIKE 'Apr/%' THEN REPLACE(en_monthYear, UPPER('Apr'), 'ABR')
                                        WHEN en_monthYear LIKE 'May/%' THEN REPLACE(en_monthYear, UPPER('May'), 'MAI')
                                        WHEN en_monthYear LIKE 'Jun/%' THEN REPLACE(en_monthYear, UPPER('Jun'), 'JUN')
                                        WHEN en_monthYear LIKE 'Jul/%' THEN REPLACE(en_monthYear, UPPER('Jul'), 'JUL')
                                        WHEN en_monthYear LIKE 'Aug/%' THEN REPLACE(en_monthYear, UPPER('Aug'), 'AGO')
                                        WHEN en_monthYear LIKE 'Sep/%' THEN REPLACE(en_monthYear, UPPER('Sep'), 'SET')
                                        WHEN en_monthYear LIKE 'Oct/%' THEN REPLACE(en_monthYear, UPPER('Oct'), 'OUT')
                                        WHEN en_monthYear LIKE 'Nov/%' THEN REPLACE(en_monthYear, UPPER('Nov'), 'NOV')
                                        WHEN en_monthYear LIKE 'Dec/%' THEN REPLACE(en_monthYear, UPPER('Dec'), 'DEZ')
                                        ELSE en_monthYear
                                    END;
                                END;");
        }
    }
}
