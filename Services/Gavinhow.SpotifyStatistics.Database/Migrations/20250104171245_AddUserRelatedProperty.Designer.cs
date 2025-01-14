﻿// <auto-generated />
using System;
using Gavinhow.SpotifyStatistics.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gavinhow.SpotifyStatistics.Database.Migrations
{
    [DbContext(typeof(SpotifyStatisticsContext))]
    [Migration("20250104171245_AddUserRelatedProperty")]
    partial class AddUserRelatedProperty
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("SpotifyTracker")
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.ArtistAlbum", b =>
                {
                    b.Property<string>("AlbumId")
                        .HasColumnType("text");

                    b.Property<string>("ArtistId")
                        .HasColumnType("text");

                    b.HasKey("AlbumId", "ArtistId");

                    b.ToTable("ArtistAlbums", "SpotifyTracker");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.ArtistTrack", b =>
                {
                    b.Property<string>("ArtistId")
                        .HasColumnType("text");

                    b.Property<string>("TrackId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ArtistId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("ArtistTracks", "SpotifyTracker");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Friend", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("FriendId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "FriendId");

                    b.ToTable("Friends", "SpotifyTracker");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Play", b =>
                {
                    b.Property<string>("TrackId")
                        .HasColumnType("text");

                    b.Property<DateTime>("TimeOfPlay")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("TrackId", "TimeOfPlay", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Plays", "SpotifyTracker");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Track", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AlbumId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Tracks", "SpotifyTracker");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AccessToken")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<double>("ExpiresIn")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime>("TokenCreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users", "SpotifyTracker");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.ArtistTrack", b =>
                {
                    b.HasOne("Gavinhow.SpotifyStatistics.Database.Entity.Track", null)
                        .WithMany("Artists")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Friend", b =>
                {
                    b.HasOne("Gavinhow.SpotifyStatistics.Database.Entity.User", "User")
                        .WithMany("Friends")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Play", b =>
                {
                    b.HasOne("Gavinhow.SpotifyStatistics.Database.Entity.Track", "Track")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Gavinhow.SpotifyStatistics.Database.Entity.User", "User")
                        .WithMany("Plays")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Track");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.Track", b =>
                {
                    b.Navigation("Artists");
                });

            modelBuilder.Entity("Gavinhow.SpotifyStatistics.Database.Entity.User", b =>
                {
                    b.Navigation("Friends");

                    b.Navigation("Plays");
                });
#pragma warning restore 612, 618
        }
    }
}