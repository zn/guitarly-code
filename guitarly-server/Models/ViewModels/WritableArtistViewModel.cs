using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Models.ViewModels
{
    public class WritableArtistViewModel
    {
        [Required(ErrorMessage = "Укажите название артиста или группы")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Slug is null")]
        [MaxLength(50)]
        public string Slug { get; set; }
        public IFormFile Image { get; set; }
        
        // each name on a new line
        public string AlternativeNames { get; set; }
    }
}