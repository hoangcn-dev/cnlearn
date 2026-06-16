using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class AddExamAndQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInBank",
                table: "Question",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    AccessType = table.Column<int>(type: "int", nullable: false),
                    IsDraft = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ContributeToBank = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsQuizSource = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.ExamId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamQuestion",
                columns: table => new
                {
                    ExamQuestionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ExamId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestion", x => x.ExamQuestionId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    TargetGroup = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    SourceType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    ExamId = table.Column<Guid>(type: "char(36)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDraft = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LockBrowser = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShuffleQuestions = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DisableCopyPaste = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Fullscreen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Webcam = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IpLimit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowLateJoin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowLateSubmit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PublicLeaderboard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SendEmailReport = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DirectCategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DirectTotalQuestions = table.Column<int>(type: "int", nullable: true),
                    DirectEasyCount = table.Column<int>(type: "int", nullable: false),
                    DirectMediumCount = table.Column<int>(type: "int", nullable: false),
                    DirectHardCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.QuizId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "ExamQuestion");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropColumn(
                name: "IsInBank",
                table: "Question");
        }
    }
}
