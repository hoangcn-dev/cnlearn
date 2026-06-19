using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueForUserSavedMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSavedQuestionMapping");

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
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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
                name: "UserSavedMapping");

            migrationBuilder.CreateTable(
                name: "UserSavedQuestionMapping",
                columns: table => new
                {
                    UserSavedQuestionMappingId = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SaveType = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedQuestionMapping", x => x.UserSavedQuestionMappingId);
                    table.ForeignKey(
                        name: "FK_UserSavedQuestionMapping_LearnMsUser_UserId",
                        column: x => x.UserId,
                        principalTable: "LearnMsUser",
                        principalColumn: "LearnMsUserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedQuestionMapping_UserId",
                table: "UserSavedQuestionMapping",
                column: "UserId");
        }
    }
}
