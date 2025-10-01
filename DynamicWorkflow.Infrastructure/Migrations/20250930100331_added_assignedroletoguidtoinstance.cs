using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_assignedroletoguidtoinstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkflowInstanceId",
                table: "WorkFlowInstanceSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "WorkflowInstances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowInstanceSteps_WorkflowInstanceId",
                table: "WorkFlowInstanceSteps",
                column: "WorkflowInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowInstanceSteps_WorkflowInstances_WorkflowInstanceId",
                table: "WorkFlowInstanceSteps",
                column: "WorkflowInstanceId",
                principalTable: "WorkflowInstances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowInstanceSteps_WorkflowInstances_WorkflowInstanceId",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlowInstanceSteps_WorkflowInstanceId",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropColumn(
                name: "WorkflowInstanceId",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "WorkflowInstances");
        }
    }
}
