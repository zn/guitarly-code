using Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    public class SearchResultViewModel
    {
        public List<ArtistEntity> Artists { get; set; }
        public List<SongEntity> Songs { get; set; }
    }
}
