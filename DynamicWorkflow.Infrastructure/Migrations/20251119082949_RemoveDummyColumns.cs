using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDummyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstances_ActionTypes_ActionTypeEntityId",
                table: "WorkflowInstances");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowInstances_ActionTypeEntityId",
                table: "WorkflowInstances");

            migrationBuilder.DropColumn(
                name: "ActionTypeEntityId",
                table: "WorkflowInstances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionTypeEntityId",
                table: "WorkflowInstances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_ActionTypeEntityId",
                table: "WorkflowInstances",
                column: "ActionTypeEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstances_ActionTypes_ActionTypeEntityId",
                table: "WorkflowInstances",
                column: "ActionTypeEntityId",
                principalTable: "ActionTypes",
                principalColumn: "Id");
        }
    }
}
