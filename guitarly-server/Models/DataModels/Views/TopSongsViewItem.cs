using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DataModels
{
    public class TopSongsViewItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullTitle { get; set; }

        public int TotalViews { get; set; }

        public int? ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
