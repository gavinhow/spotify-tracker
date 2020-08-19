using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gavinhow.SpotifyStatistics.Database.Migrations
{
    public partial class Add_user_and_play_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SpotifyTracker");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "SpotifyTracker",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    AccessToken = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    ExpiresIn = table.Column<double>(nullable: false),
                    TokenCreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plays",
                schema: "SpotifyTracker",
                columns: table => new
                {
                    TrackId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    TimeOfPlay = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plays", x => new { x.TrackId, x.TimeOfPlay, x.UserId });
                    table.ForeignKey(
                        name: "FK_Plays_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "SpotifyTracker",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plays_UserId",
                schema: "SpotifyTracker",
                table: "Plays",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plays",
                schema: "SpotifyTracker");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "SpotifyTracker");
        }
    }
}
