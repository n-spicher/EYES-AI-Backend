using Microsoft.EntityFrameworkCore.Migrations;

namespace Pdf_Processor.Migrations
{
    public partial class CategoryItem_Note : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "CategoryItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "CategoryItems");
        }
    }
}
