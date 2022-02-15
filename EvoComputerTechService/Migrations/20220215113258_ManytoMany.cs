using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EvoComputerTechService.Migrations
{
    public partial class ManytoMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Issues_IssueId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IssueId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "IssueProducts",
                columns: table => new
                {
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueProducts", x => new { x.IssueId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_IssueProducts_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueProducts_ProductId",
                table: "IssueProducts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueProducts");

            migrationBuilder.AddColumn<Guid>(
                name: "IssueId",
                table: "Products",
                type: "uniqueidentifier",
                maxLength: 450,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_IssueId",
                table: "Products",
                column: "IssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Issues_IssueId",
                table: "Products",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
