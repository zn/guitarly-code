using System;
using System.Linq;
using AutoMapper;
using Models.DataModels;
using Models.EntityModels;
using Models.ViewModels;

namespace Api
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            CreateMap<Song, SongEntity>().ReverseMap();
            CreateMap<Artist, ArtistEntity>().ReverseMap();
            CreateMap<ArtistAlternativeName, ArtistAlternativeNameEntity>().ReverseMap();
            CreateMap<User, UserEntity>().ReverseMap();

            CreateMap<ArtistEntity, WritableArtistViewModel>();
            CreateMap<WritableArtistViewModel, ArtistEntity>();

            CreateMap<ArtistAlternativeNameEntity, ArtistAlternativeNameViewModel>().ReverseMap();

            CreateMap<SongEntity, WritableSongViewModel>().ReverseMap();
            CreateMap<SongEntity, EditSongViewModel>().ReverseMap();
        }
    }
}
