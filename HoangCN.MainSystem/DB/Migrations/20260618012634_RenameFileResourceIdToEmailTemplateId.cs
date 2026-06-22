using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.MainSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenameFileResourceIdToEmailTemplateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thay thế bằng SQL thô để tránh bug không tìm thấy metadata cột cũ trong RenameColumn của MySql.EntityFrameworkCore.
            migrationBuilder.Sql("ALTER TABLE EmailTemplate CHANGE COLUMN FileResourceId EmailTemplateId char(36) NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE EmailTemplate CHANGE COLUMN EmailTemplateId FileResourceId char(36) NOT NULL;");
        }
    }
}
