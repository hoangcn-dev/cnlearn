using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionCategoryIdToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionInCategory");

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionCategoryId",
                table: "Question",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionCategoryId",
                table: "Question");

            migrationBuilder.CreateTable(
                name: "QuestionInCategory",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    QuestionCategoryId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OrderInList = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionInCategory", x => new { x.QuestionId, x.QuestionCategoryId });
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }
    }
}
