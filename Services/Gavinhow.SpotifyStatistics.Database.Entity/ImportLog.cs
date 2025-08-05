using System;
using System.ComponentModel.DataAnnotations;

namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class ImportLog : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime ImportDateTime { get; set; }

        public int TracksImported { get; set; }

        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public User User { get; set; }
    }
}