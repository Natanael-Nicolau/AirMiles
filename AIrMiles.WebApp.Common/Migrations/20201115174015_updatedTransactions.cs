using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIrMiles.WebApp.Common.Migrations
{
    public partial class updatedTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<int>(
                name: "CreditCardId",
                table: "Transactions",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BaseMilesPrice",
                table: "Flights",
                nullable: false,
                oldClrType: typeof(decimal));  
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreditCardId",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BaseMilesPrice",
                table: "Flights",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
