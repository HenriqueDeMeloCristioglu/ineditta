using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SET @dbname = DATABASE();
                SET @tablename = 'acompanhanto_cct';
                SET @columnname = 'grupos_economicos_ids';
                SET @columntype = 'JSON';
                SET @ddl = CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname, ' ', @columntype, ' NULL;');
                SELECT @exists := COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE table_schema = @dbname 
                AND table_name = @tablename 
                AND column_name = @columnname;
                SET @stmt = IF(@exists > 0, 'SELECT ''column exists'';', @ddl);
                PREPARE stmt FROM @stmt;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;");

            migrationBuilder.Sql(@"SET @dbname = DATABASE();
                SET @tablename = 'acompanhanto_cct';
                SET @columnname = 'empresas_ids';
                SET @columntype = 'JSON';
                SET @ddl = CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname, ' ', @columntype, ' NULL;');
                SELECT @exists := COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE table_schema = @dbname 
                AND table_name = @tablename 
                AND column_name = @columnname;
                SET @stmt = IF(@exists > 0, 'SELECT ''column exists'';', @ddl);
                PREPARE stmt FROM @stmt;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                SET @dbname = DATABASE();
                SET @tablename = 'acompanhanto_cct';
                SET @columnname = 'grupos_economicos_ids';
                SET @ddl = CONCAT('ALTER TABLE ', @tablename, ' DROP COLUMN ', @columnname, ';');
                SELECT @exists := COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE table_schema = @dbname 
                AND table_name = @tablename 
                AND column_name = @columnname;

                SET @stmt = IF(@exists > 0, @ddl, 'SELECT ''column does not exist'';');
                PREPARE stmt FROM @stmt;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql($@"
                SET @dbname = DATABASE();
                SET @tablename = 'acompanhanto_cct';
                SET @columnname = 'empresas_ids';
                SET @ddl = CONCAT('ALTER TABLE ', @tablename, ' DROP COLUMN ', @columnname, ';');
                SELECT @exists := COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE table_schema = @dbname 
                AND table_name = @tablename 
                AND column_name = @columnname;

                SET @stmt = IF(@exists > 0, @ddl, 'SELECT ''column does not exist'';');
                PREPARE stmt FROM @stmt;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");
        }
    }
}
