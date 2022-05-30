using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.DataModels;
using Models.EntityModels;

namespace Models.DataModels
{
    public class TopArtistsViewItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Picture30 { get; set; }
        public string Picture100 { get; set; }
        public string PictureOriginal { get; set; }
        public int TotalViews { get; set; }
    }
}