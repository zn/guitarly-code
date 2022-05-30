using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DataModels
{
    [Table("artist_alternative_names")]
    public class ArtistAlternativeName
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}