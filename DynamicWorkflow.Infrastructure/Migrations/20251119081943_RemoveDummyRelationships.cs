using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDummyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_ApplicationRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ApplicationRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationRoleId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ApplicationRoleId",
                table: "AspNetUsers",
                column: "ApplicationRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_ApplicationRoleId",
                table: "AspNetUsers",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }
    }
}
