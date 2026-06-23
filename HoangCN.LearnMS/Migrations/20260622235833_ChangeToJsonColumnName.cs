using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToJsonColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `Question` CHANGE COLUMN `CorrectKeys` `CorrectKeysJsonData` LONGTEXT NULL;");
            migrationBuilder.Sql("ALTER TABLE `Question` CHANGE COLUMN `Answers` `AnswersJsonData` LONGTEXT NULL;");
            migrationBuilder.Sql("ALTER TABLE `ExamQuestion` CHANGE COLUMN `CorrectKeys` `CorrectKeysJsonData` LONGTEXT NULL;");
            migrationBuilder.Sql("ALTER TABLE `ExamQuestion` CHANGE COLUMN `Answers` `AnswersJsonData` LONGTEXT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `Question` CHANGE COLUMN `CorrectKeysJsonData` `CorrectKeys` LONGTEXT NULL;");
            migrationBuilder.Sql("ALTER TABLE `Question` CHANGE COLUMN `AnswersJsonData` `Answers` LONGTEXT NULL;");
            migrationBuilder.Sql("ALTER TABLE `ExamQuestion` CHANGE COLUMN `CorrectKeysJsonData` `CorrectKeys` LONGTEXT NULL;");
            migrationBuilder.Sql("ALTER TABLE `ExamQuestion` CHANGE COLUMN `AnswersJsonData` `Answers` LONGTEXT NULL;");
        }
    }
}
