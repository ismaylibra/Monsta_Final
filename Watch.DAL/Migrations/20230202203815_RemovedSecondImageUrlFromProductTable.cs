using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Watch.DAL.Migrations
{
    public partial class RemovedSecondImageUrlFromProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstImageUrl",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "SecondImageUrl",
                table: "Products",
                newName: "MainImageUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainImageUrl",
                table: "Products",
                newName: "SecondImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "FirstImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
