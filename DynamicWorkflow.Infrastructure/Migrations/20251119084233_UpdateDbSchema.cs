using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowInstanceSteps_ActionTypes_ActionTypeEntityId",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlowInstanceSteps_ActionTypeEntityId",
                table: "WorkFlowInstanceSteps");

            migrationBuilder.DropColumn(
                name: "ActionTypeEntityId",
                table: "WorkFlowInstanceSteps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionTypeEntityId",
                table: "WorkFlowInstanceSteps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowInstanceSteps_ActionTypeEntityId",
                table: "WorkFlowInstanceSteps",
                column: "ActionTypeEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowInstanceSteps_ActionTypes_ActionTypeEntityId",
                table: "WorkFlowInstanceSteps",
                column: "ActionTypeEntityId",
                principalTable: "ActionTypes",
                principalColumn: "Id");
        }
    }
}
