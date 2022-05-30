using System;
using System.Collections.Generic;
using System.Text;
using Models.DataModels;
using Models.EntityModels;

namespace Models.ViewModels
{
    public class HomePageViewModel
    {
        public List<SongEntity> UnpublishedSongs { get; set; }
        public List<SongEntity> LatestSongs { get; set; }
        public List<TopArtistsViewItem> TopArtists { get; set; }
        public List<TopSongsViewItem> TopSongs { get; set; }
    }
}
