using Microsoft.EntityFrameworkCore.Migrations;

namespace AIrMiles.WebApp.Common.Migrations
{
    public partial class UpdatedClientEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProlongedMiles",
                table: "Clients",
                newName: "ExtendedMiles");

            migrationBuilder.AddColumn<int>(
                name: "ConvertedMiles",
                table: "Clients",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertedMiles",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "ExtendedMiles",
                table: "Clients",
                newName: "ProlongedMiles");
        }
    }
}
