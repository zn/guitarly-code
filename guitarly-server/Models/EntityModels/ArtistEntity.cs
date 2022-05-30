using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.DataModels;

namespace Models.EntityModels
{
    public class ArtistEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Picture30 { get; set; }
        public string Picture100 { get; set; }
        public string PictureOriginal { get; set; }
        public int ViewsNumber { get; set; }

        public IList<SongEntity> Songs{ get; set; }
        public IList<ArtistAlternativeNameEntity> ArtistAlternativeNameEntities { get; set; }
    }
}