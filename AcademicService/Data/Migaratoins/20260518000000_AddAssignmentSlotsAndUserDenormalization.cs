using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicService.Data.Migaratoins
{
    public partial class AddAssignmentSlotsAndUserDenormalization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Assignments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "DATEADD(day, 7, GETUTCDATE())");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Assignments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "AssignmentSubmissions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentFirstName",
                table: "AssignmentSubmissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentFullName",
                table: "AssignmentSubmissions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "CourseEnrollments",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentFirstName",
                table: "CourseEnrollments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentFullName",
                table: "CourseEnrollments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorEmail",
                table: "Courses",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorFirstName",
                table: "Courses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorFullName",
                table: "Courses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CourseId", table: "Assignments");
            migrationBuilder.DropColumn(name: "Deadline", table: "Assignments");
            migrationBuilder.DropColumn(name: "Instructions", table: "Assignments");
            migrationBuilder.DropColumn(name: "IsOpen", table: "Assignments");
            migrationBuilder.DropColumn(name: "StudentEmail", table: "AssignmentSubmissions");
            migrationBuilder.DropColumn(name: "StudentFirstName", table: "AssignmentSubmissions");
            migrationBuilder.DropColumn(name: "StudentFullName", table: "AssignmentSubmissions");
            migrationBuilder.DropColumn(name: "StudentEmail", table: "CourseEnrollments");
            migrationBuilder.DropColumn(name: "StudentFirstName", table: "CourseEnrollments");
            migrationBuilder.DropColumn(name: "StudentFullName", table: "CourseEnrollments");
            migrationBuilder.DropColumn(name: "DoctorEmail", table: "Courses");
            migrationBuilder.DropColumn(name: "DoctorFirstName", table: "Courses");
            migrationBuilder.DropColumn(name: "DoctorFullName", table: "Courses");
        }
    }
}
