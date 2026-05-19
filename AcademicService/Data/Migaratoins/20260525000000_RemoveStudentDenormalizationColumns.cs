using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicService.Data.Migaratoins
{
    [Migration("20260525000000_RemoveStudentDenormalizationColumns")]
    public partial class RemoveStudentDenormalizationColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DropColumnIfExists(migrationBuilder, "AssignmentSubmissions", "StudentEmail");
            DropColumnIfExists(migrationBuilder, "AssignmentSubmissions", "StudentFirstName");
            DropColumnIfExists(migrationBuilder, "AssignmentSubmissions", "StudentFullName");
            DropColumnIfExists(migrationBuilder, "CourseEnrollments", "StudentEmail");
            DropColumnIfExists(migrationBuilder, "CourseEnrollments", "StudentFirstName");
            DropColumnIfExists(migrationBuilder, "CourseEnrollments", "StudentFullName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            AddStringColumnIfMissing(migrationBuilder, "AssignmentSubmissions", "StudentEmail", "nvarchar(256)");
            AddStringColumnIfMissing(migrationBuilder, "AssignmentSubmissions", "StudentFirstName", "nvarchar(100)");
            AddStringColumnIfMissing(migrationBuilder, "AssignmentSubmissions", "StudentFullName", "nvarchar(200)");
            AddStringColumnIfMissing(migrationBuilder, "CourseEnrollments", "StudentEmail", "nvarchar(256)");
            AddStringColumnIfMissing(migrationBuilder, "CourseEnrollments", "StudentFirstName", "nvarchar(100)");
            AddStringColumnIfMissing(migrationBuilder, "CourseEnrollments", "StudentFullName", "nvarchar(200)");
        }

        private static void DropColumnIfExists(MigrationBuilder migrationBuilder, string table, string column)
        {
            migrationBuilder.Sql($"""
                IF COL_LENGTH(N'{table}', N'{column}') IS NOT NULL
                BEGIN
                    DECLARE @constraintName nvarchar(128);
                    SELECT @constraintName = dc.name
                    FROM sys.default_constraints dc
                    INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                    INNER JOIN sys.tables t ON t.object_id = c.object_id
                    WHERE t.name = N'{table}' AND c.name = N'{column}';

                    IF @constraintName IS NOT NULL
                        EXEC(N'ALTER TABLE [{table}] DROP CONSTRAINT [' + @constraintName + N']');

                    ALTER TABLE [{table}] DROP COLUMN [{column}];
                END
                """);
        }

        private static void AddStringColumnIfMissing(MigrationBuilder migrationBuilder, string table, string column, string type)
        {
            migrationBuilder.Sql($"""
                IF COL_LENGTH(N'{table}', N'{column}') IS NULL
                    ALTER TABLE [{table}] ADD [{column}] {type} NOT NULL CONSTRAINT [DF_{table}_{column}] DEFAULT N'';
                """);
        }
    }
}
