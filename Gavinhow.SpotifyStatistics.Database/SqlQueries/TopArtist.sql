SELECT ArtistTracks.ArtistId, COUNT(ArtistTracks.ArtistId) AS Count
FROM SpotifyTracker.Plays AS Plays
INNER JOIN SpotifyTracker.Users AS Users ON Plays.UserId = Users.Id
LEFT JOIN SpotifyTracker.ArtistTracks As ArtistTracks ON ArtistTracks.TrackId = Plays.TrackId
WHERE Users.DisplayName LIKE '%' AND Plays.TrackId LIKE '%'
GROUP BY ArtistTracks.ArtistId
ORDER BY Count DESC;