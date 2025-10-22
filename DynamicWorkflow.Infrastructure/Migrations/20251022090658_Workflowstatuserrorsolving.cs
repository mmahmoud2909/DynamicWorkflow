using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Workflowstatuserrorsolving : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workflows_WorkflowStatuses_WorkflowStatusId",
                table: "Workflows");

            migrationBuilder.AddColumn<int>(
                name: "WorkflowStatusId1",
                table: "Workflows",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_WorkflowStatusId1",
                table: "Workflows",
                column: "WorkflowStatusId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Workflows_WorkflowStatuses_WorkflowStatusId",
                table: "Workflows",
                column: "WorkflowStatusId",
                principalTable: "WorkflowStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workflows_WorkflowStatuses_WorkflowStatusId1",
                table: "Workflows",
                column: "WorkflowStatusId1",
                principalTable: "WorkflowStatuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workflows_WorkflowStatuses_WorkflowStatusId",
                table: "Workflows");

            migrationBuilder.DropForeignKey(
                name: "FK_Workflows_WorkflowStatuses_WorkflowStatusId1",
                table: "Workflows");

            migrationBuilder.DropIndex(
                name: "IX_Workflows_WorkflowStatusId1",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "WorkflowStatusId1",
                table: "Workflows");

            migrationBuilder.AddForeignKey(
                name: "FK_Workflows_WorkflowStatuses_WorkflowStatusId",
                table: "Workflows",
                column: "WorkflowStatusId",
                principalTable: "WorkflowStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
