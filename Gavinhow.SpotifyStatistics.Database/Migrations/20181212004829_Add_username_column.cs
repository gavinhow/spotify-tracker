using Microsoft.EntityFrameworkCore.Migrations;

namespace Gavinhow.SpotifyStatistics.Database.Migrations
{
    public partial class Add_username_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "SpotifyTracker",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "SpotifyTracker",
                table: "Users");
        }
    }
}
