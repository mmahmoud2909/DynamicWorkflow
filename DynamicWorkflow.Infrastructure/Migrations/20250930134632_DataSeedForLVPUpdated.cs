using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataSeedForLVPUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_StepRole_StepId",
                table: "StepRole",
                column: "StepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepRole");

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

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "WorkflowInstances",
                type: "int",
                nullable: false,
                defaultValue: 4,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 5);
        }
    }
}
