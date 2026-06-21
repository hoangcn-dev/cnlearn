using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsQuizSource",
                table: "Exam");

            migrationBuilder.Sql("ALTER TABLE Exam RENAME COLUMN Duration TO DurationMin;");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Question",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceExamId",
                table: "Question",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DraftData",
                table: "Exam",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "SourceExamId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "DraftData",
                table: "Exam");

            migrationBuilder.Sql("ALTER TABLE Exam RENAME COLUMN DurationMin TO Duration;");

            migrationBuilder.AddColumn<bool>(
                name: "IsQuizSource",
                table: "Exam",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
