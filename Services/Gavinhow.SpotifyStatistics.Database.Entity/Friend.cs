namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class Friend : BaseEntity
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        
        public User User { get; set; }
    }
}