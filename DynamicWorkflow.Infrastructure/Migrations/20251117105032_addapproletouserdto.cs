using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addapproletouserdto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "APPRoleId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_APPRoleId",
                table: "AspNetUsers",
                column: "APPRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AppRoles_APPRoleId",
                table: "AspNetUsers",
                column: "APPRoleId",
                principalTable: "AppRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AppRoles_APPRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_APPRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "APPRoleId",
                table: "AspNetUsers");
        }
    }
}
