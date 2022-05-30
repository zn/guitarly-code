using Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    public class SongViewModel
    {
        public SongEntity Song { get; set; }
        public List<SongEntity> Recommendations { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsPublished { get; set; }
    }
}
