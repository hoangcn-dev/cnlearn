using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class ReInit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LearnMsUser",
                columns: table => new
                {
                    LearnMsUserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FullName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Biography = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnMsUser", x => x.LearnMsUserId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionCategory",
                columns: table => new
                {
                    QuestionCategoryId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuestionCategorySlug = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    QuestionCategoryName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCategory", x => x.QuestionCategoryId);
                    table.ForeignKey(
                        name: "FK_QuestionCategory_QuestionCategory_ParentId",
                        column: x => x.ParentId,
                        principalTable: "QuestionCategory",
                        principalColumn: "QuestionCategoryId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserSavedMapping",
                columns: table => new
                {
                    UserSavedMappingId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TargetId = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    SaveType = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedMapping", x => x.UserSavedMappingId);
                    table.ForeignKey(
                        name: "FK_UserSavedMapping_LearnMsUser_UserId",
                        column: x => x.UserId,
                        principalTable: "LearnMsUser",
                        principalColumn: "LearnMsUserId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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
                    table.ForeignKey(
                        name: "FK_Exam_LearnMsUser_UserId",
                        column: x => x.UserId,
                        principalTable: "LearnMsUser",
                        principalColumn: "LearnMsUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exam_QuestionCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "QuestionCategory",
                        principalColumn: "QuestionCategoryId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuestionSlug = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    StringContent = table.Column<string>(type: "longtext", nullable: true),
                    Explaination = table.Column<string>(type: "longtext", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    LearnMsUserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    AccessType = table.Column<int>(type: "int", nullable: false),
                    IsInBank = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QuestionCategoryId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Question_LearnMsUser_LearnMsUserId",
                        column: x => x.LearnMsUserId,
                        principalTable: "LearnMsUser",
                        principalColumn: "LearnMsUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Question_QuestionCategory_QuestionCategoryId",
                        column: x => x.QuestionCategoryId,
                        principalTable: "QuestionCategory",
                        principalColumn: "QuestionCategoryId",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_Quiz_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quiz_LearnMsUser_UserId",
                        column: x => x.UserId,
                        principalTable: "LearnMsUser",
                        principalColumn: "LearnMsUserId",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_ExamQuestion_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQuestion_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionAnswer",
                columns: table => new
                {
                    QuestionAnswerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StringContent = table.Column<string>(type: "longtext", nullable: true),
                    IsCorrectAnswer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    OrderInList = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
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
                name: "QuestionMedia",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Label = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    OrderInList = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionMedia", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_QuestionMedia_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamAttempt",
                columns: table => new
                {
                    ExamAttemptId = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    AttemptType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ExamId = table.Column<Guid>(type: "char(36)", nullable: true),
                    QuizId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    CorrectCount = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FinishedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAttempt", x => x.ExamAttemptId);
                    table.ForeignKey(
                        name: "FK_ExamAttempt_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamAttempt_LearnMsUser_UserId",
                        column: x => x.UserId,
                        principalTable: "LearnMsUser",
                        principalColumn: "LearnMsUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamAttempt_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamSession",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CandidateId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuizId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastHeartbeatAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BlurCount = table.Column<int>(type: "int", nullable: false),
                    FullscreenExitCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSession", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_ExamSession_LearnMsUser_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "LearnMsUser",
                        principalColumn: "LearnMsUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSession_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionAnswerMedia",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuestionAnswerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Label = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    OrderInList = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ExamAttemptDetail",
                columns: table => new
                {
                    ExamAttemptDetailId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ExamAttemptId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    SelectedAnswerIds = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    IsCorrect = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAttemptDetail", x => x.ExamAttemptDetailId);
                    table.ForeignKey(
                        name: "FK_ExamAttemptDetail_ExamAttempt_ExamAttemptId",
                        column: x => x.ExamAttemptId,
                        principalTable: "ExamAttempt",
                        principalColumn: "ExamAttemptId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamAttemptDetail_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamCheatLog",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "char(36)", nullable: false),
                    SessionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ViolationType = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCheatLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_ExamCheatLog_ExamSession_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ExamSession",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_CategoryId",
                table: "Exam",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_UserId",
                table: "Exam",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempt_ExamId",
                table: "ExamAttempt",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempt_QuizId",
                table: "ExamAttempt",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempt_UserId",
                table: "ExamAttempt",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttemptDetail_ExamAttemptId",
                table: "ExamAttemptDetail",
                column: "ExamAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttemptDetail_QuestionId",
                table: "ExamAttemptDetail",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCheatLog_SessionId",
                table: "ExamCheatLog",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_ExamId_QuestionId",
                table: "ExamQuestion",
                columns: new[] { "ExamId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestion_QuestionId",
                table: "ExamQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSession_CandidateId_QuizId",
                table: "ExamSession",
                columns: new[] { "CandidateId", "QuizId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSession_QuizId",
                table: "ExamSession",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LearnMsUser_Email",
                table: "LearnMsUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_LearnMsUserId",
                table: "Question",
                column: "LearnMsUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionCategoryId",
                table: "Question",
                column: "QuestionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionSlug",
                table: "Question",
                column: "QuestionSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_QuestionId",
                table: "QuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerMedia_QuestionAnswerId",
                table: "QuestionAnswerMedia",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCategory_ParentId",
                table: "QuestionCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCategory_QuestionCategorySlug",
                table: "QuestionCategory",
                column: "QuestionCategorySlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionMedia_QuestionId",
                table: "QuestionMedia",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_ExamId",
                table: "Quiz",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_UserId",
                table: "Quiz",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedMapping_UserId_TargetId",
                table: "UserSavedMapping",
                columns: new[] { "UserId", "TargetId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamAttemptDetail");

            migrationBuilder.DropTable(
                name: "ExamCheatLog");

            migrationBuilder.DropTable(
                name: "ExamQuestion");

            migrationBuilder.DropTable(
                name: "QuestionAnswerMedia");

            migrationBuilder.DropTable(
                name: "QuestionMedia");

            migrationBuilder.DropTable(
                name: "UserSavedMapping");

            migrationBuilder.DropTable(
                name: "ExamAttempt");

            migrationBuilder.DropTable(
                name: "ExamSession");

            migrationBuilder.DropTable(
                name: "QuestionAnswer");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "LearnMsUser");

            migrationBuilder.DropTable(
                name: "QuestionCategory");
        }
    }
}
