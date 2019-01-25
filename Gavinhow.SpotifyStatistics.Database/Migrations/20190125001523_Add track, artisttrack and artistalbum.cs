using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gavinhow.SpotifyStatistics.Database.Migrations
{
    public partial class Addtrackartisttrackandartistalbum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArtistAlbums",
                schema: "SpotifyTracker",
                columns: table => new
                {
                    ArtistId = table.Column<string>(nullable: false),
                    AlbumId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistAlbums", x => new { x.AlbumId, x.ArtistId });
                });

            migrationBuilder.CreateTable(
                name: "ArtistTracks",
                schema: "SpotifyTracker",
                columns: table => new
                {
                    ArtistId = table.Column<string>(nullable: false),
                    TrackId = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistTracks", x => new { x.ArtistId, x.TrackId });
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                schema: "SpotifyTracker",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    AlbumId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistAlbums",
                schema: "SpotifyTracker");

            migrationBuilder.DropTable(
                name: "ArtistTracks",
                schema: "SpotifyTracker");

            migrationBuilder.DropTable(
                name: "Tracks",
                schema: "SpotifyTracker");
        }
    }
}
