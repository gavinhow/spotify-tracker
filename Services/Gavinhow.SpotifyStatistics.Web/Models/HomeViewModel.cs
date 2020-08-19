using System.Collections.Generic;
using Gavinhow.SpotifyStatistics.Api.Models;
using Gavinhow.SpotifyStatistics.Database.Entity;

namespace Gavinhow.SpotifyStatistics.Web.Models
{
    public class HomeViewModel
    {
        public User CurrentUser { get; set; }
        public readonly bool HasPlays;
        public readonly Play OldestSong;
        public readonly List<SongPlayCount> MostPlayedSongs;
        public readonly List<SongPlayCount> LastMonthMostPlayedSongs;
        public readonly List<ArtistPlayCount> MostPlayedArtists;

        public readonly int LastMonthPlayCount;
        public readonly int TotalPlayCount;

        public HomeViewModel(User user)
        {
            CurrentUser = user;
            HasPlays = false;
        }

        public HomeViewModel(User user, Play oldestSong, List<SongPlayCount> mostPlayedSongs, List<ArtistPlayCount> mostPlayedArtists, List<SongPlayCount> lastMonthPlayed, int lastMonthPlayCount, int totalPlayCount)
        {
            CurrentUser = user;
            OldestSong = oldestSong;
            MostPlayedSongs = mostPlayedSongs;
            MostPlayedArtists = mostPlayedArtists;
            LastMonthMostPlayedSongs = lastMonthPlayed;
            LastMonthPlayCount = lastMonthPlayCount;
            TotalPlayCount = totalPlayCount;
            
            HasPlays = (oldestSong != null);
        }
    }
}
