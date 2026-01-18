using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixParentStudentRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ParentStudents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ParentStudents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ParentStudents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId1",
                table: "ParentStudents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId1",
                table: "ParentStudents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ParentStudents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParentStudents_ParentId1",
                table: "ParentStudents",
                column: "ParentId1");

            migrationBuilder.CreateIndex(
                name: "IX_ParentStudents_StudentId1",
                table: "ParentStudents",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudents_Users_ParentId1",
                table: "ParentStudents",
                column: "ParentId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudents_Users_StudentId1",
                table: "ParentStudents",
                column: "StudentId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudents_Users_ParentId1",
                table: "ParentStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudents_Users_StudentId1",
                table: "ParentStudents");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_ParentStudents_ParentId1",
                table: "ParentStudents");

            migrationBuilder.DropIndex(
                name: "IX_ParentStudents_StudentId1",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "ParentId1",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ParentStudents");
        }
    }
}
