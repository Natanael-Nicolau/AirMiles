using Microsoft.EntityFrameworkCore.Migrations;

namespace AIrMiles.WebApp.Common.Migrations
{
    public partial class MilesRequestFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_milesRequests_Clients_ClientId",
                table: "milesRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_milesRequests_Partners_PartnerId",
                table: "milesRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_milesRequests",
                table: "milesRequests");

            migrationBuilder.DropColumn(
                name: "BirtDate",
                table: "Clients");

            migrationBuilder.RenameTable(
                name: "milesRequests",
                newName: "MilesRequests");

            migrationBuilder.RenameIndex(
                name: "IX_milesRequests_PartnerId",
                table: "MilesRequests",
                newName: "IX_MilesRequests_PartnerId");

            migrationBuilder.RenameIndex(
                name: "IX_milesRequests_ClientId",
                table: "MilesRequests",
                newName: "IX_MilesRequests_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MilesRequests",
                table: "MilesRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MilesRequests_Clients_ClientId",
                table: "MilesRequests",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MilesRequests_Partners_PartnerId",
                table: "MilesRequests",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MilesRequests_Clients_ClientId",
                table: "MilesRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MilesRequests_Partners_PartnerId",
                table: "MilesRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MilesRequests",
                table: "MilesRequests");

            migrationBuilder.RenameTable(
                name: "MilesRequests",
                newName: "milesRequests");

            migrationBuilder.RenameIndex(
                name: "IX_MilesRequests_PartnerId",
                table: "milesRequests",
                newName: "IX_milesRequests_PartnerId");

            migrationBuilder.RenameIndex(
                name: "IX_MilesRequests_ClientId",
                table: "milesRequests",
                newName: "IX_milesRequests_ClientId");

            migrationBuilder.AddColumn<int>(
                name: "BirtDate",
                table: "Clients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_milesRequests",
                table: "milesRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_milesRequests_Clients_ClientId",
                table: "milesRequests",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_milesRequests_Partners_PartnerId",
                table: "milesRequests",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
