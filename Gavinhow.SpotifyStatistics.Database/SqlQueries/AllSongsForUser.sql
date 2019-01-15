SELECT Users.DisplayName, Plays.TrackId, Plays.TimeOfPlay
FROM SpotifyTracker.Plays AS Plays
INNER JOIN SpotifyTracker.Users AS Users ON Plays.UserId = Users.Id
WHERE Users.DisplayName LIKE '%' AND TrackId LIKE '%'
ORDER BY TimeOfPlay DESC;