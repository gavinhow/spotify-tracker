﻿// <auto-generated />
using System;
using Gavinhow.SpotifyStatistics.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Gavinhow.SpotifyStatistics.Database.Migrations
{
    [DbContext(typeof(SpotifyStatisticsContext))]
    [Migration("20190125001523_Add track, artisttrack and artistalbum")]
    partial class Addtrackartisttrackandartistalbum
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("SpotifyTracker")
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.ArtistAlbum", b =>
                {
                    b.Property<string>("AlbumId");

                    b.Property<string>("ArtistId");

                    b.HasKey("AlbumId", "ArtistId");

                    b.ToTable("ArtistAlbums");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.ArtistTrack", b =>
                {
                    b.Property<string>("ArtistId");

                    b.Property<string>("TrackId");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Modified");

                    b.HasKey("ArtistId", "TrackId");

                    b.ToTable("ArtistTracks");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Play", b =>
                {
                    b.Property<string>("TrackId");

                    b.Property<DateTime>("TimeOfPlay");

                    b.Property<string>("UserId");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Modified");

                    b.HasKey("TrackId", "TimeOfPlay", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Plays");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Track", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AlbumId");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Modified");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken");

                    b.Property<DateTime>("Created");

                    b.Property<string>("DisplayName");

                    b.Property<double>("ExpiresIn");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("RefreshToken");

                    b.Property<DateTime>("TokenCreateDate");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Play", b =>
                {
                    b.HasOne("Gavinhow.SpotifyStatistics.Database.Entity.User", "User")
                        .WithMany("Plays")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
