using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduManagementLab.EfRepository.Migrations
{
    public partial class ChangedCourseLineItemtoCourseTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItemResults_CourseLineItems_CourseLineItemId",
                table: "LineItemResults");

            migrationBuilder.DropTable(
                name: "CourseLineItems");

            migrationBuilder.RenameColumn(
                name: "CourseLineItemId",
                table: "LineItemResults",
                newName: "CourseTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_LineItemResults_CourseLineItemId",
                table: "LineItemResults",
                newName: "IX_LineItemResults_CourseTaskId");

            migrationBuilder.CreateTable(
                name: "CourseTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTasks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTasks_CourseId",
                table: "CourseTasks",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItemResults_CourseTasks_CourseTaskId",
                table: "LineItemResults",
                column: "CourseTaskId",
                principalTable: "CourseTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItemResults_CourseTasks_CourseTaskId",
                table: "LineItemResults");

            migrationBuilder.DropTable(
                name: "CourseTasks");

            migrationBuilder.RenameColumn(
                name: "CourseTaskId",
                table: "LineItemResults",
                newName: "CourseLineItemId");

            migrationBuilder.RenameIndex(
                name: "IX_LineItemResults_CourseTaskId",
                table: "LineItemResults",
                newName: "IX_LineItemResults_CourseLineItemId");

            migrationBuilder.CreateTable(
                name: "CourseLineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseLineItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseLineItems_CourseId",
                table: "CourseLineItems",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItemResults_CourseLineItems_CourseLineItemId",
                table: "LineItemResults",
                column: "CourseLineItemId",
                principalTable: "CourseLineItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
