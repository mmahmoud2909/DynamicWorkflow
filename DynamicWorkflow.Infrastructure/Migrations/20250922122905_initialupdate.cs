using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstanceStatus",
                table: "WorkflowInstances");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                table: "WorkflowTransitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FromState",
                table: "WorkflowTransitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PerformedBy",
                table: "WorkflowTransitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "WorkflowTransitions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ToState",
                table: "WorkflowTransitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowInstanceId",
                table: "WorkflowTransitions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "WorkflowInstances",
                type: "int",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_WorkflowInstanceId",
                table: "WorkflowTransitions",
                column: "WorkflowInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowTransitions_WorkflowInstances_WorkflowInstanceId",
                table: "WorkflowTransitions",
                column: "WorkflowInstanceId",
                principalTable: "WorkflowInstances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowTransitions_WorkflowInstances_WorkflowInstanceId",
                table: "WorkflowTransitions");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowTransitions_WorkflowInstanceId",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "FromState",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "PerformedBy",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "ToState",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "WorkflowInstanceId",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "State",
                table: "WorkflowInstances");

            migrationBuilder.AddColumn<int>(
                name: "InstanceStatus",
                table: "WorkflowInstances",
                type: "int",
                nullable: false,
                defaultValue: 3);
        }
    }
}
