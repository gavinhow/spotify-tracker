SELECT Tracks.AlbumId, COUNT(Tracks.AlbumId) AS Count
FROM SpotifyTracker.Plays AS Plays
INNER JOIN SpotifyTracker.Users AS Users ON Plays.UserId = Users.Id
LEFT JOIN SpotifyTracker.Tracks As Tracks ON Tracks.Id = Plays.TrackId
WHERE Users.DisplayName LIKE '%' AND Plays.TrackId LIKE '%'
GROUP BY Tracks.AlbumId
ORDER BY Count DESC;