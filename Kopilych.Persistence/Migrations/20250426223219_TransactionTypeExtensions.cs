using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kopilych.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TransactionTypeExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPositive",
                table: "TransactionTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPositive",
                table: "TransactionTypes");
        }
    }
}
