using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DataModels
{
    [Table("songs")]
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullTitle { get; set; }

        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int ViewsNumber { get; set; }
        public string CreatedUserId { get; set; }
        public User CreatedUser { get; set; }

        public string PublishedUserId { get; set; }
        public User PublishedUser { get; set; }

        public int? ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
