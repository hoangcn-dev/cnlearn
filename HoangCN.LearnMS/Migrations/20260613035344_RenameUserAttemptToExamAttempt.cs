using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserAttemptToExamAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserAttempt",
                newName: "ExamAttempt");

            migrationBuilder.RenameTable(
                name: "UserAttemptDetail",
                newName: "ExamAttemptDetail");

            migrationBuilder.RenameColumn(
                name: "UserAttemptId",
                table: "ExamAttempt",
                newName: "ExamAttemptId");

            migrationBuilder.RenameColumn(
                name: "UserAttemptDetailId",
                table: "ExamAttemptDetail",
                newName: "ExamAttemptDetailId");

            migrationBuilder.RenameColumn(
                name: "UserAttemptId",
                table: "ExamAttemptDetail",
                newName: "ExamAttemptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ExamAttempt",
                newName: "UserAttempt");

            migrationBuilder.RenameTable(
                name: "ExamAttemptDetail",
                newName: "UserAttemptDetail");

            migrationBuilder.RenameColumn(
                name: "ExamAttemptId",
                table: "UserAttempt",
                newName: "UserAttemptId");

            migrationBuilder.RenameColumn(
                name: "ExamAttemptDetailId",
                table: "UserAttemptDetail",
                newName: "UserAttemptDetailId");

            migrationBuilder.RenameColumn(
                name: "ExamAttemptId",
                table: "UserAttemptDetail",
                newName: "UserAttemptId");
        }
    }
}
