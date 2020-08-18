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
    [Migration("20200704205523_AddFriendsTable")]
    partial class AddFriendsTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("SpotifyTracker")
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.ArtistAlbum", b =>
                {
                    b.Property<string>("AlbumId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ArtistId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AlbumId", "ArtistId");

                    b.ToTable("ArtistAlbums");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.ArtistTrack", b =>
                {
                    b.Property<string>("ArtistId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TrackId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.HasKey("ArtistId", "TrackId");

                    b.ToTable("ArtistTracks");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Friend", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FriendId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "FriendId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Play", b =>
                {
                    b.Property<string>("TrackId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TimeOfPlay")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.HasKey("TrackId", "TimeOfPlay", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Plays");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Track", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AlbumId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ExpiresIn")
                        .HasColumnType("float");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime2");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TokenCreateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Friend", b =>
                {
                    b.HasOne("Gavinhow.SpotifyStatistics.Database.Entity.User", null)
                        .WithMany("Friends")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Play", b =>
                {
                    b.HasOne("Gavinhow.SpotifyStatistics.Database.Entity.User", "User")
                        .WithMany("Plays")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}