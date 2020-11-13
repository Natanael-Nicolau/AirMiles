using Microsoft.EntityFrameworkCore.Migrations;

namespace AIrMiles.WebApp.Common.Migrations
{
    public partial class GifterFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GifterId",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GifterId",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
