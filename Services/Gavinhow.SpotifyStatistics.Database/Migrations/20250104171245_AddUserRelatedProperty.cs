using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gavinhow.SpotifyStatistics.Database.Migrations
{
    public partial class AddUserRelatedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "TrackId",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ArtistId",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistTracks_TrackId",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistTracks_Tracks_TrackId",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                column: "TrackId",
                principalSchema: "SpotifyTracker",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistTracks_Tracks_TrackId",
                schema: "SpotifyTracker",
                table: "ArtistTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Plays_Tracks_TrackId",
                schema: "SpotifyTracker",
                table: "Plays");

            migrationBuilder.DropIndex(
                name: "IX_ArtistTracks_TrackId",
                schema: "SpotifyTracker",
                table: "ArtistTracks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "TrackId",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ArtistId",
                schema: "SpotifyTracker",
                table: "ArtistTracks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
