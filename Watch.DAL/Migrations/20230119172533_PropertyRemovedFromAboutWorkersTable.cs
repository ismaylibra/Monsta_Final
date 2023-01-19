using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watch.DAL.Migrations
{
    public partial class PropertyRemovedFromAboutWorkersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "AboutWorkers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AboutWorkers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
