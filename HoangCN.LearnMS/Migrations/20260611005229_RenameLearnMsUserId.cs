using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.LearnMS.Migrations
{
    /// <inheritdoc />
    public partial class RenameLearnMsUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE LearnMsUser RENAME COLUMN UserId TO LearnMsUserId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE LearnMsUser RENAME COLUMN LearnMsUserId TO UserId;");
        }
    }
}
