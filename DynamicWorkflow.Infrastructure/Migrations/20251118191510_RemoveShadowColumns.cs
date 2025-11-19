using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AppRoles_AppRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AppRoles_AppRoleId1",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowTransitions_ActionTypes_ActionTypeEntityId1",
                table: "WorkflowTransitions");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowTransitions_ActionTypeEntityId1",
                table: "WorkflowTransitions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AppRoleId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ActionTypeEntityId1",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "AppRoleId1",
                table: "AspNetUsers");

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

            migrationBuilder.AddColumn<int>(
                name: "ActionTypeEntityId1",
                table: "WorkflowTransitions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppRoleId1",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_ActionTypeEntityId1",
                table: "WorkflowTransitions",
                column: "ActionTypeEntityId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AppRoleId1",
                table: "AspNetUsers",
                column: "AppRoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AppRoles_AppRoleId",
                table: "AspNetUsers",
                column: "AppRoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AppRoles_AppRoleId1",
                table: "AspNetUsers",
                column: "AppRoleId1",
                principalTable: "AppRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowTransitions_ActionTypes_ActionTypeEntityId1",
                table: "WorkflowTransitions",
                column: "ActionTypeEntityId1",
                principalTable: "ActionTypes",
                principalColumn: "Id");
        }
    }
}
