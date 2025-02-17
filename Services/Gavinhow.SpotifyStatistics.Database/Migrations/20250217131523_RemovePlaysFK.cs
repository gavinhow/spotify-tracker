using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gavinhow.SpotifyStatistics.Database.Migrations
{
    public partial class RemovePlaysFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plays_Tracks_TrackId",
                schema: "SpotifyTracker",
                table: "Plays");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Plays_Tracks_TrackId",
                schema: "SpotifyTracker",
                table: "Plays",
                column: "TrackId",
                principalSchema: "SpotifyTracker",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
