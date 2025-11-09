using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowTransitions_WorkflowStatuses_WorkflowStatusId",
                table: "WorkflowTransitions");

            migrationBuilder.DropTable(
                name: "StepRoles");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceActions");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowTransitions_WorkflowStatusId",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "FromState",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "ToState",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "WorkflowStatusId",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "AssignedRole",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "stepActionTypes",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "stepStatus",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropColumn(
                name: "State",
                table: "WorkflowInstances");

            migrationBuilder.RenameColumn(
                name: "Condition",
                table: "WorkflowSteps",
                newName: "UpdatedBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkflowTransitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "WorkflowTransitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkflowSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformedBy",
                table: "WorkflowSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WorkflowStatusId",
                table: "Workflows",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "Workflows",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Workflows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Workflows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkFlowInstanceSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "WorkFlowInstanceSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformedBy",
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "PerformedBy",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkflowInstances");

            migrationBuilder.DropColumn(
                name: "PerformedBy",
                table: "WorkflowInstances");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "WorkflowInstances");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "WorkflowSteps",
                newName: "Condition");

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

            migrationBuilder.AddColumn<int>(
                name: "ToState",
                table: "WorkflowTransitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowStatusId",
                table: "WorkflowTransitions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedRole",
                table: "WorkflowSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "stepActionTypes",
                table: "WorkflowSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "stepStatus",
                table: "WorkflowSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "WorkflowStatusId",
                table: "Workflows",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "Workflows",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Workflows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "WorkFlowInstanceSteps",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "WorkflowInstances",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "StepRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    ActorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepRoles_WorkflowSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstanceActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionTypeEntityId = table.Column<int>(type: "int", nullable: false),
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: false),
                    WorkFlowInstanceStepId = table.Column<int>(type: "int", nullable: false),
                    WorkflowStatusId = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkflowStepId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceActions_ActionTypes_ActionTypeEntityId",
                        column: x => x.ActionTypeEntityId,
                        principalTable: "ActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceActions_WorkFlowInstanceSteps_WorkFlowInstanceStepId",
                        column: x => x.WorkFlowInstanceStepId,
                        principalTable: "WorkFlowInstanceSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceActions_WorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceActions_WorkflowStatuses_WorkflowStatusId",
                        column: x => x.WorkflowStatusId,
                        principalTable: "WorkflowStatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_WorkflowStatusId",
                table: "WorkflowTransitions",
                column: "WorkflowStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StepRoles_StepId",
                table: "StepRoles",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceActions_ActionTypeEntityId",
                table: "WorkflowInstanceActions",
                column: "ActionTypeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceActions_WorkflowInstanceId",
                table: "WorkflowInstanceActions",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceActions_WorkFlowInstanceStepId",
                table: "WorkflowInstanceActions",
                column: "WorkFlowInstanceStepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceActions_WorkflowStatusId",
                table: "WorkflowInstanceActions",
                column: "WorkflowStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowTransitions_WorkflowStatuses_WorkflowStatusId",
                table: "WorkflowTransitions",
                column: "WorkflowStatusId",
                principalTable: "WorkflowStatuses",
                principalColumn: "Id");
        }
    }
}
