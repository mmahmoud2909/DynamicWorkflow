using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LVPNeededUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StepRole_WorkflowSteps_StepId",
                table: "StepRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StepRole",
                table: "StepRole");

            migrationBuilder.RenameTable(
                name: "StepRole",
                newName: "StepRoles");

            migrationBuilder.RenameIndex(
                name: "IX_StepRole_StepId",
                table: "StepRoles",
                newName: "IX_StepRoles_StepId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StepRoles",
                table: "StepRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StepRoles_WorkflowSteps_StepId",
                table: "StepRoles",
                column: "StepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StepRoles_WorkflowSteps_StepId",
                table: "StepRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StepRoles",
                table: "StepRoles");

            migrationBuilder.RenameTable(
                name: "StepRoles",
                newName: "StepRole");

            migrationBuilder.RenameIndex(
                name: "IX_StepRoles_StepId",
                table: "StepRole",
                newName: "IX_StepRole_StepId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StepRole",
                table: "StepRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StepRole_WorkflowSteps_StepId",
                table: "StepRole",
                column: "StepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
