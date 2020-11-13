using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIrMiles.WebApp.Common.Migrations
{
    public partial class AddedMilesRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BaseMilesPrice",
                table: "Flights",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.CreateTable(
                name: "milesRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsAproved = table.Column<bool>(nullable: false),
                    RequestCode = table.Column<string>(nullable: true),
                    ClientId = table.Column<int>(nullable: false),
                    MilesAmount = table.Column<int>(nullable: false),
                    PartnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_milesRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_milesRequests_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_milesRequests_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_milesRequests_ClientId",
                table: "milesRequests",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_milesRequests_PartnerId",
                table: "milesRequests",
                column: "PartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "milesRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "BaseMilesPrice",
                table: "Flights",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
