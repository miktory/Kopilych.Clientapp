using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kopilych.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PiggyBankTypePhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstStatePhotoPath",
                table: "PiggyBankTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FourthStatePhotoPath",
                table: "PiggyBankTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondStatePhotoPath",
                table: "PiggyBankTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdStatePhotoPath",
                table: "PiggyBankTypes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstStatePhotoPath",
                table: "PiggyBankTypes");

            migrationBuilder.DropColumn(
                name: "FourthStatePhotoPath",
                table: "PiggyBankTypes");

            migrationBuilder.DropColumn(
                name: "SecondStatePhotoPath",
                table: "PiggyBankTypes");

            migrationBuilder.DropColumn(
                name: "ThirdStatePhotoPath",
                table: "PiggyBankTypes");
        }
    }
}
