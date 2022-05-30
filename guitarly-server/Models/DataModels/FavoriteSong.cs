using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Models.DataModels
{
    public class FavoriteSong
    {
        public int SongId { get; set; }
        public Song Song { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime AddedDate { get; set; }
    }
}