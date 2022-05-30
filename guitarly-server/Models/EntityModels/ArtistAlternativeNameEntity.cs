using System.ComponentModel.DataAnnotations.Schema;

namespace Models.EntityModels
{
    public class ArtistAlternativeNameEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public ArtistEntity Artist { get; set; }
    }
}