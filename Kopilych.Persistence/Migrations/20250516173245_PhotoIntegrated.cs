using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kopilych.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PhotoIntegrated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PhotoIntegrated",
                table: "PiggyBankCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoIntegrated",
                table: "PiggyBankCustomizations");
        }
    }
}
