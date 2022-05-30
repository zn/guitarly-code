using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ArtistViewsNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "views_number",
                table: "artists",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "views_number",
                table: "artists");
        }
    }
}
