using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kopilych.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "UserPiggyBanks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "PiggyBanks",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "UserPiggyBanks");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "PiggyBanks");
        }
    }
}
