using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInstanceActionandconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "comments",
                table: "WorkflowSteps",
                newName: "Comments");

            migrationBuilder.RenameColumn(
                name: "stepName",
                table: "WorkflowSteps",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Workflows",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Workflows",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "WorkflowSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Workflows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentWorkflowId",
                table: "Workflows",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "WorkflowInstances",
                type: "int",
                nullable: false,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 4);

            migrationBuilder.CreateTable(
                name: "StepRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepRole_WorkflowSteps_StepId",
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
                    WorkflowInstanceId = table.Column<int>(type: "int", nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkFlowInstanceStepId = table.Column<int>(type: "int", nullable: false),
                    WorkflowStepId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstanceActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceActions_WorkFlowInstanceSteps_WorkFlowInstanceStepId",
                        column: x => x.WorkFlowInstanceStepId,
                        principalTable: "WorkFlowInstanceSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowInstanceActions_WorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StepRole_StepId",
                table: "StepRole",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceActions_WorkflowInstanceId",
                table: "WorkflowInstanceActions",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstanceActions_WorkFlowInstanceStepId",
                table: "WorkflowInstanceActions",
                column: "WorkFlowInstanceStepId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepRole");

            migrationBuilder.DropTable(
                name: "WorkflowInstanceActions");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "ParentWorkflowId",
                table: "Workflows");

            migrationBuilder.RenameColumn(
                name: "Comments",
                table: "WorkflowSteps",
                newName: "comments");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "WorkflowSteps",
                newName: "stepName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Workflows",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Workflows",
                newName: "description");

            migrationBuilder.AddColumn<int>(
                name: "WorkflowInstanceId",
                table: "WorkFlowInstanceSteps",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "WorkflowInstances",
                type: "int",
                nullable: false,
                defaultValue: 4,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 5);

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
    }
}
