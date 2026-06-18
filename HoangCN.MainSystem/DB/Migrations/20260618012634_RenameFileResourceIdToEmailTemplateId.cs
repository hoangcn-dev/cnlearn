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
            migrationBuilder.RenameColumn(
                name: "FileResourceId",
                table: "EmailTemplate",
                newName: "EmailTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailTemplateId",
                table: "EmailTemplate",
                newName: "FileResourceId");
        }
    }
}
