using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoangCN.DL.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailTemplatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    FileResourceId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TemplateCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "longtext", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.FileResourceId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_TemplateCode",
                table: "EmailTemplate",
                column: "TemplateCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTemplate");
        }
    }
}
