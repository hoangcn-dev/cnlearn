using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameOfCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `Question` DROP FOREIGN KEY `FK_Question_QuestionCategory_QuestionCategoryId`;");
            migrationBuilder.Sql("ALTER TABLE `Question` DROP INDEX `IX_Question_QuestionCategoryId`;");
            migrationBuilder.Sql("ALTER TABLE `LearnMsUser` DROP COLUMN `Role`;");
            migrationBuilder.Sql("ALTER TABLE `QuestionCategory` RENAME COLUMN `Slug` TO `QuestionCategorySlug`;");
            migrationBuilder.Sql("ALTER TABLE `QuestionCategory` RENAME COLUMN `Name` TO `QuestionCategoryName`;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `QuestionCategory` RENAME COLUMN `QuestionCategorySlug` TO `Slug`;");
            migrationBuilder.Sql("ALTER TABLE `QuestionCategory` RENAME COLUMN `QuestionCategoryName` TO `Name`;");
            migrationBuilder.Sql("ALTER TABLE `LearnMsUser` ADD COLUMN `Role` varchar(50) NULL;");
            migrationBuilder.Sql("CREATE INDEX `IX_Question_QuestionCategoryId` ON `Question` (`QuestionCategoryId`);");
            migrationBuilder.Sql("ALTER TABLE `Question` ADD CONSTRAINT `FK_Question_QuestionCategory_QuestionCategoryId` FOREIGN KEY (`QuestionCategoryId`) REFERENCES `QuestionCategory` (`QuestionCategoryId`) ON DELETE CASCADE;");
        }
    }
}
