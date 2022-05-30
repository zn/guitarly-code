using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class UserFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "asp_net_users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "asp_net_users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo100",
                table: "asp_net_users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo200",
                table: "asp_net_users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo_max_orig",
                table: "asp_net_users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sex",
                table: "asp_net_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_name",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "photo100",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "photo200",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "photo_max_orig",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "sex",
                table: "asp_net_users");
        }
    }
}
