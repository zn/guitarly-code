using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.ViewModels
{
    public class ArtistAlternativeNameViewModel
    {
        [MaxLength(200)]
        public string Title { get; set; }
    }
}