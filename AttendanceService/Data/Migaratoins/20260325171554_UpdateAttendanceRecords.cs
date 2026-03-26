using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceService.Data.Migaratoins
{
    /// <inheritdoc />
    public partial class UpdateAttendanceRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAttended",
                table: "AttendanceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LectureTitle",
                table: "AttendanceRecords",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAttended",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LectureTitle",
                table: "AttendanceRecords");
        }
    }
}
