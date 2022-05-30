using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.DataModels;
using Models.EntityModels;

namespace Models.ViewModels
{
    public class WritableSongViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string FullTitle { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Text { get; set; }

        public int? ArtistId { get; set; }
    }
}
