using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSaveAnswerMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionAnswerMedia");

            migrationBuilder.DropTable(
                name: "QuestionAnswer");

            migrationBuilder.AddColumn<string>(
                name: "Answers",
                table: "Question",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "CorrectKeys",
                table: "Question",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answers",
                table: "ExamQuestion",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "CorrectKeys",
                table: "ExamQuestion",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answers",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "CorrectKeys",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "Answers",
                table: "ExamQuestion");

            migrationBuilder.DropColumn(
                name: "CorrectKeys",
                table: "ExamQuestion");

            migrationBuilder.CreateTable(
                name: "QuestionAnswer",
                columns: table => new
                {
                    QuestionAnswerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExamQuestionId = table.Column<Guid>(type: "char(36)", nullable: true),
                    IsCorrectAnswer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OrderInList = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: true),
                    StringContent = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswer", x => x.QuestionAnswerId);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionAnswerMedia",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Label = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OrderInList = table.Column<int>(type: "int", nullable: false),
                    QuestionAnswerId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswerMedia", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_QuestionAnswerMedia_QuestionAnswer_QuestionAnswerId",
                        column: x => x.QuestionAnswerId,
                        principalTable: "QuestionAnswer",
                        principalColumn: "QuestionAnswerId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_QuestionId",
                table: "QuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerMedia_QuestionAnswerId",
                table: "QuestionAnswerMedia",
                column: "QuestionAnswerId");
        }
    }
}
