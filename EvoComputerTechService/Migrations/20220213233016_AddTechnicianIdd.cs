using Microsoft.EntityFrameworkCore.Migrations;

namespace EvoComputerTechService.Migrations
{
    public partial class AddTechnicianIdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "Issues",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TechnicianId",
                table: "Issues",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_AspNetUsers_TechnicianId",
                table: "Issues",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_AspNetUsers_TechnicianId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TechnicianId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "Issues");
        }
    }
}
