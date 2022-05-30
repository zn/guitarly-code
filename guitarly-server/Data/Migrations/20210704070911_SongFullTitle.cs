using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class SongFullTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "full_title",
                table: "songs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_title",
                table: "songs");
        }
    }
}
