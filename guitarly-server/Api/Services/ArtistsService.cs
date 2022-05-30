using System;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Microsoft.Extensions.Logging;
using Models.ViewModels;
using Models.DataModels;
using Models.EntityModels;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ApplicationCore;

namespace Api.Services
{
    public class ArtistsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ArtistsService> _logger;
        private readonly FilesService _filesService;
        private readonly ArtistsViewsHistory _artistsViewsHistory;
        
        public ArtistsService(
            ILogger<ArtistsService> logger,
            AppDbContext context,
            IMapper mapper,
            ArtistsViewsHistory artistsViewsHistory,
            FilesService filesService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _filesService = filesService;
            _artistsViewsHistory = artistsViewsHistory;
        }

        public IList<ArtistEntity> GetAll()
        {
            return _mapper.Map<IList<ArtistEntity>>(_context.Artists.ToList());
        }

        public ArtistEntity GetById(int id, string userId)
        {
            var artist = _context.Artists.Include(x => x.Songs).FirstOrDefault(x => x.Id == id);

            if(!_artistsViewsHistory.Contains(userId, id))
            {
                artist.ViewsNumber++;
                _context.Artists.Update(artist);
                _context.SaveChanges();
                _artistsViewsHistory.Add(userId, id);
            }

            artist.Songs = artist.Songs.Where(x => !x.IsDeleted && x.PublishedAt != null).OrderByDescending(x => x.ViewsNumber).ToList();
            foreach (var song in artist.Songs)
            {
                song.Text = null;
            }
            return _mapper.Map<ArtistEntity>(artist);
        }

        public int AddArtist(WritableArtistViewModel model)
        {
            var pictures  = _filesService.SaveArtistPicture(model); // array of 3. First - original, second - 30x30, third - 100x100
            var artist = new Artist{
                Title = model.Title,
                ViewsNumber = 0,
                PictureOriginal = pictures[0],
                Picture30 = pictures[1],
                Picture100 = pictures[2]
            };
            var entry = _context.Artists.Add(artist);
            _context.SaveChanges();

            if(!string.IsNullOrEmpty(model.AlternativeNames))
            {
                var alternativeNames = model.AlternativeNames
                    .Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                    .Select(x => new ArtistAlternativeName
                    {
                        ArtistId = entry.Entity.Id,
                        Title = x
                    }).ToList();
                _context.ArtistAlternativeNames.AddRange(alternativeNames);
                _context.SaveChanges();
            }
            return entry.Entity.Id;
        }

        public List<TopArtistsViewItem> GetTopArtists(int page = 1)
        {
            return _context.TopArtists
                .OrderByDescending(x => x.TotalViews)
                .Skip((page-1) * SettingsConstants.ARTISTS_PAGE_SIZE)
                .Take(SettingsConstants.ARTISTS_PAGE_SIZE)
                .ToList();
        }

        public AddSongViewModel GetAddSongModel()
        {
            var result = new AddSongViewModel();
            result.ArtistsList = _mapper.Map<List<ArtistEntity>>(_context.Artists.OrderBy(x=>x.Title).ToList());
            return result;
        }
    }
}
