using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class AddUniversityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniversityId",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                WITH CTE AS (
                    SELECT Id, ROW_NUMBER() OVER(ORDER BY Id) as row_num
                    FROM Users
                )
                UPDATE U
                SET U.UniversityId = RIGHT(CAST(YEAR(GETDATE()) AS VARCHAR), 2) + RIGHT('0000' + CAST(C.row_num AS VARCHAR), 4)
                FROM Users U
                JOIN CTE C ON U.Id = C.Id
                WHERE U.UniversityId = ''
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UniversityId",
                table: "Users",
                column: "UniversityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UniversityId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Users");
        }
    }
}
