namespace Gavinhow.SpotifyStatistics.Web.Settings;

public class TableStorageSettings
{
    public string ConnectionString { get; set; } = "";
    public string TableName { get; set; } = "Users";
    public int SyncIntervalSeconds { get; set; } = 300;
    public bool Enabled { get; set; } = true;
}
