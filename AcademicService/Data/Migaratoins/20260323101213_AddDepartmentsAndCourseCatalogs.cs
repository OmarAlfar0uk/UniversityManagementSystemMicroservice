using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicService.Data.Migaratoins
{
    /// <inheritdoc />
    public partial class AddDepartmentsAndCourseCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_Assignments_AssignmentId",
                table: "AssignmentSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturePdfs_Lectures_LectureId",
                table: "LecturePdfs");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Courses_CourseId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureVideos_Lectures_LectureId",
                table: "LectureVideos");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseCatalogId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCatalogId",
                table: "Courses",
                column: "CourseCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalogs_Code",
                table: "CourseCatalogs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Code",
                table: "Departments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_Assignments_AssignmentId",
                table: "AssignmentSubmissions",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseCatalogs_CourseCatalogId",
                table: "Courses",
                column: "CourseCatalogId",
                principalTable: "CourseCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturePdfs_Lectures_LectureId",
                table: "LecturePdfs",
                column: "LectureId",
                principalTable: "Lectures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Courses_CourseId",
                table: "Lectures",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureVideos_Lectures_LectureId",
                table: "LectureVideos",
                column: "LectureId",
                principalTable: "Lectures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_Assignments_AssignmentId",
                table: "AssignmentSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseCatalogs_CourseCatalogId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturePdfs_Lectures_LectureId",
                table: "LecturePdfs");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Courses_CourseId",
                table: "Lectures");

            migrationBuilder.DropForeignKey(
                name: "FK_LectureVideos_Lectures_LectureId",
                table: "LectureVideos");

            migrationBuilder.DropTable(
                name: "CourseCatalogs");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CourseCatalogId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseCatalogId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_Assignments_AssignmentId",
                table: "AssignmentSubmissions",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                table: "CourseEnrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturePdfs_Lectures_LectureId",
                table: "LecturePdfs",
                column: "LectureId",
                principalTable: "Lectures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Courses_CourseId",
                table: "Lectures",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LectureVideos_Lectures_LectureId",
                table: "LectureVideos",
                column: "LectureId",
                principalTable: "Lectures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
