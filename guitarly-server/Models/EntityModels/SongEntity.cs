using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.DataModels;

namespace Models.EntityModels
{
    public class SongEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string FullTitle { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int ViewsNumber { get; set; }
        public int? CreatedUserId { get; set; }
        public UserEntity CreatedUser { get; set; }

        public string PublishedUserId { get; set; }
        public User PublishedUser { get; set; }

        public int? ArtistId { get; set; }
        public ArtistEntity Artist { get; set; }
    }
}
