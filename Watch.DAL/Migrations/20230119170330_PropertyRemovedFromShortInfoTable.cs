using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watch.DAL.Migrations
{
    public partial class PropertyRemovedFromShortInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "whyUsShortInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "whyUsShortInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
