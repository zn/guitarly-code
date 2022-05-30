using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DataModels
{
    [Table("artists")]
    public class Artist
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Picture30 { get; set; }
        public string Picture100 { get; set; }
        public string PictureOriginal { get; set; }
        public int ViewsNumber { get; set; }

        public IList<Song> Songs { get; set; }
        public IList<ArtistAlternativeName> ArtistAlternativeNames { get; set; }
    }
}