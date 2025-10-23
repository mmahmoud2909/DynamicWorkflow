using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowInstanceStatusText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusText",
                table: "WorkflowInstances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusText",
                table: "WorkflowInstances");
        }
    }
}
