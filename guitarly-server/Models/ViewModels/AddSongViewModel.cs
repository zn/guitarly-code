using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.EntityModels;
using System.Collections.Generic;

namespace Models.ViewModels
{
    public class AddSongViewModel
    {
        public List<ArtistEntity> ArtistsList { get; set; }
    }
}