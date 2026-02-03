using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Models;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Web.Services;

public class PlaysQueueProcessor : IPlaysQueueProcessor
{
    private readonly SpotifyStatisticsContext _dbContext;
    private readonly SpotifyApiFacade _spotifyApiFacade;
    private readonly SpotifySettings _spotifySettings;
    private readonly ILogger<PlaysQueueProcessor> _logger;

    public PlaysQueueProcessor(
        SpotifyStatisticsContext dbContext,
        SpotifyApiFacade spotifyApiFacade,
        IOptions<SpotifySettings> spotifySettings,
        ILogger<PlaysQueueProcessor> logger)
    {
        _dbContext = dbContext;
        _spotifyApiFacade = spotifyApiFacade;
        _spotifySettings = spotifySettings.Value;
        _logger = logger;
    }

    public async Task ProcessPlaysAsync(UserPlaysQueueMessage message, CancellationToken ct)
    {
        var userId = message.UserId;

        _logger.LogInformation(
            "Processing {PlayCount} plays for user {UserId} (window: {Start} - {End})",
            message.Plays.Count, userId, message.ImportWindowStart, message.ImportWindowEnd);

        // Get existing plays within the timeframe to minimize duplicate check query
        var existingPlaysInRange = _dbContext.Plays
            .Where(p => p.UserId == userId
                && p.TimeOfPlay >= message.ImportWindowStart
                && p.TimeOfPlay <= message.ImportWindowEnd)
            .Select(p => new { p.TrackId, p.TimeOfPlay })
            .ToHashSet();

        var newPlays = new List<Play>();

        foreach (var play in message.Plays)
        {
            var playKey = new { play.TrackId, play.TimeOfPlay };

            if (!existingPlaysInRange.Contains(playKey))
            {
                newPlays.Add(new Play
                {
                    TrackId = play.TrackId,
                    UserId = userId,
                    TimeOfPlay = play.TimeOfPlay
                });
            }
        }

        if (newPlays.Count > 0)
        {
            _dbContext.Plays.AddRange(newPlays);
        }

        int recordsUpdated = await _dbContext.SaveChangesAsync();
        _logger.LogInformation("{RecordsUpdated} records saved for user {UserId}", recordsUpdated, userId);

        await LogImportAttempt(userId, newPlays.Count, true, null);

        // Import missing track metadata if not disabled
        if (!_spotifySettings.DisableMetadataFetching)
        {
            await ImportMissingTrackMetadataAsync(ct);
        }
    }

    private async Task ImportMissingTrackMetadataAsync(CancellationToken ct)
    {
        var results = (from plays in _dbContext.Plays
            join tracks in _dbContext.Tracks
                on plays.TrackId equals tracks.Id into joined
            from j in joined.DefaultIfEmpty()
            where j.Id == null
            select plays.TrackId).Distinct();

        List<string> trackIds = results.ToList();

        if (trackIds.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Found {TrackCount} tracks with no metadata", trackIds.Count);

        try
        {
            for (int i = 0; i < trackIds.Count && !ct.IsCancellationRequested; i += 50)
            {
                var batch = trackIds.Skip(i).Take(50).ToList();
                foreach (var item in await _spotifyApiFacade.GetSeveralTracksAsync(batch))
                {
                    SaveTrackInformation(item);
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Track metadata import completed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to import track metadata, plays were still saved");
        }
    }

    private void SaveTrackInformation(FullTrack item)
    {
        Track track = new Track
        {
            Id = item.Id,
            AlbumId = item.Album.Id
        };

        _dbContext.Tracks.AddIfNotExists(track, x => x.Id == item.Id);

        foreach (var artist in item.Artists)
        {
            ArtistTrack artistTrack = new ArtistTrack
            {
                TrackId = item.Id,
                ArtistId = artist.Id
            };
            _dbContext.ArtistTracks.AddIfNotExists(artistTrack,
                x => x.ArtistId == artistTrack.ArtistId
                     && x.TrackId == artistTrack.TrackId);
        }

        foreach (var artist in item.Album.Artists)
        {
            ArtistAlbum artistAlbum = new ArtistAlbum
            {
                ArtistId = artist.Id,
                AlbumId = item.Album.Id
            };
            _dbContext.ArtistAlbums.AddIfNotExists(artistAlbum,
                x => x.AlbumId == artistAlbum.AlbumId
                     && x.ArtistId == artistAlbum.ArtistId);
        }
    }

    private async Task LogImportAttempt(string userId, int tracksImported, bool isSuccessful, string? errorMessage)
    {
        var importLog = new ImportLog
        {
            UserId = userId,
            ImportDateTime = DateTime.UtcNow,
            TracksImported = tracksImported,
            IsSuccessful = isSuccessful,
            ErrorMessage = errorMessage
        };

        _dbContext.ImportLogs.Add(importLog);
        await _dbContext.SaveChangesAsync();
    }
}
