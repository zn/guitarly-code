using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.EntityModels;
using Models.ViewModels;
using ApplicationCore;

namespace Api.Services
{
    public class HomeService
    {
        private readonly ILogger<HomeService> _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ArtistsService _artistsService;
        private readonly SongsService _songsService;
        public HomeService(SongsService songsService, ArtistsService artistsService, ILogger<HomeService> logger, AppDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _songsService = songsService;
            _artistsService = artistsService;
        }

        public HomePageViewModel GetPageModel(string userRole)
        {
            var model = new HomePageViewModel();

            if(userRole == RolesConstants.ADMIN || userRole == RolesConstants.MODER)
            {
                model.UnpublishedSongs = _songsService.GetUnpublishedSongs(1, true);
            }

            model.TopArtists = _artistsService.GetTopArtists(1);
            model.TopSongs = _songsService.GetTopSongs(1, true);
            model.LatestSongs = _songsService.GetLatestSongs(1, true);
            return model;
        }

        public SearchResultViewModel Search(string query)
        {
            _logger.LogInformation($"The search query is [{query}]");
            var result = new SearchResultViewModel();

            query = query.Trim().ToLower();
            var artists = _context.Artists.Include(x => x.ArtistAlternativeNames)
                .Where(x => x.Title.ToLower().Contains(query) 
                        || x.ArtistAlternativeNames.Any(n => n.Title.ToLower().Contains(query)))
                .Take(5)
                .ToList();
            
            var songs = _context.Songs
                                .Where(x => !x.IsDeleted && x.PublishedAt != null)
                                .Where(x => x.FullTitle.ToLower().Contains(query))
                                .Take(10)
                                .ToList();
            foreach(var song in songs)
            {
                song.Text = null;
            }

            result.Artists = _mapper.Map<List<ArtistEntity>>(artists);
            result.Songs = _mapper.Map<List<SongEntity>>(songs);

            return result;
        }
    }
}
