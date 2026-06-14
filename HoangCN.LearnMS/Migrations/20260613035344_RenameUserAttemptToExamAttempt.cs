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

            migrationBuilder.Sql("ALTER TABLE `ExamAttempt` RENAME COLUMN `UserAttemptId` TO `ExamAttemptId`;");
            migrationBuilder.Sql("ALTER TABLE `ExamAttemptDetail` RENAME COLUMN `UserAttemptDetailId` TO `ExamAttemptDetailId`;");
            migrationBuilder.Sql("ALTER TABLE `ExamAttemptDetail` RENAME COLUMN `UserAttemptId` TO `ExamAttemptId`;");
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

            migrationBuilder.Sql("ALTER TABLE `UserAttempt` RENAME COLUMN `ExamAttemptId` TO `UserAttemptId`;");
            migrationBuilder.Sql("ALTER TABLE `UserAttemptDetail` RENAME COLUMN `ExamAttemptDetailId` TO `UserAttemptDetailId`;");
            migrationBuilder.Sql("ALTER TABLE `UserAttemptDetail` RENAME COLUMN `ExamAttemptId` TO `UserAttemptId`;");
        }
    }
}
