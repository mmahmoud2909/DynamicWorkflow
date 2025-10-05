using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

public partial class FixWorkflowStepsRename : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Rename comments -> Comments if 'comments' exists
        migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.WorkflowSteps', 'comments') IS NOT NULL
BEGIN
    EXEC sp_rename N'dbo.WorkflowSteps.comments', N'Comments', 'COLUMN';
END
");

        // Rename stepName -> Name only if stepName exists and Name does not exist already
        migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.WorkflowSteps', 'stepName') IS NOT NULL AND COL_LENGTH('dbo.WorkflowSteps', 'Name') IS NULL
BEGIN
    EXEC sp_rename N'dbo.WorkflowSteps.stepName', N'Name', 'COLUMN';
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // reverse safely
        migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.WorkflowSteps', 'Name') IS NOT NULL AND COL_LENGTH('dbo.WorkflowSteps', 'stepName') IS NULL
BEGIN
    EXEC sp_rename N'dbo.WorkflowSteps.Name', N'stepName', 'COLUMN';
END
");

        migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.WorkflowSteps', 'Comments') IS NOT NULL AND COL_LENGTH('dbo.WorkflowSteps', 'comments') IS NULL
BEGIN
    EXEC sp_rename N'dbo.WorkflowSteps.Comments', N'comments', 'COLUMN';
END
");
    }
}
