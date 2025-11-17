using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Approleidadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AppRoles_APPRoleId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "APPRoleId",
                table: "AspNetUsers",
                newName: "AppRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_APPRoleId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_AppRoleId");

            migrationBuilder.AlterColumn<int>(
                name: "AppRoleId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AppRoles_AppRoleId",
                table: "AspNetUsers",
                column: "AppRoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AppRoles_AppRoleId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AppRoleId",
                table: "AspNetUsers",
                newName: "APPRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_AppRoleId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_APPRoleId");

            migrationBuilder.AlterColumn<int>(
                name: "APPRoleId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AppRoles_APPRoleId",
                table: "AspNetUsers",
                column: "APPRoleId",
                principalTable: "AppRoles",
                principalColumn: "Id");
        }
    }
}
