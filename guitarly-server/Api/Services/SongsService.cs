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
using System.Text.RegularExpressions;
using ApplicationCore;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Services
{
    public class SongsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SongsService> _logger;
        private readonly SongsViewsHistory _songsViewsHistory;

        public SongsService(
            ILogger<SongsService> logger,
            AppDbContext context,
            SongsViewsHistory songsViewsHistory,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _songsViewsHistory = songsViewsHistory;
        }

        public SongViewModel GetById(int id, string userId)
        {
            var song = _context.Songs.Include(x => x.Artist).FirstOrDefault(x => x.Id == id);
            if(song == null || song.IsDeleted)
            {
                return null;
            }
            
            if(!_songsViewsHistory.Contains(userId, id))
            {
                song.ViewsNumber++;
                _context.Songs.Update(song);
                _context.SaveChanges();
                _songsViewsHistory.Add(userId, id);
            }

            song.Text = formatLyrics(song.Text);
            var songEntity= _mapper.Map<SongEntity>(song);

            var recommendations = _mapper.Map<List<SongEntity>>(
                _context.Songs.Where(x => x.Id != id && x.ArtistId == song.ArtistId && !x.IsDeleted && x.PublishedAt != null).Take(5).ToList());

            foreach (var rec in recommendations)
            {
                rec.Text = null;
            }
            bool isFavorite = _context.FavoriteSongs.Any(x=>x.UserId== userId && x.SongId == id);

            return new SongViewModel
            {
                Song = songEntity,
                Recommendations = recommendations,
                IsFavorite = isFavorite,
                IsPublished = songEntity.PublishedAt != null
            };
        }

        public async Task<int> AddSong(WritableSongViewModel model)
        {
            var entity = _mapper.Map<SongEntity>(model);
            entity.CreatedAt = DateTime.UtcNow;

            entity.Text = Regex.Replace(entity.Text, @"([ABCDEFGH][#-+\d|mmaj|maj|dim|sus|b|o|aug|add|verm]*(\/\d.)?)($|\s|[.?!)(,:\/])", "<chord>$1</chord>$3");

            var entry = _context.Songs.Add(_mapper.Map<Song>(entity));
            await _context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public EditSongViewModel GetEditSongViewModel(int songId)
        {
            var song = _context.Songs.FirstOrDefault(x => x.Id == songId);
            return new EditSongViewModel
            {
                Title = song.Title,
                FullTitle = song.FullTitle,
                Text = song.Text
            };
        }

        public async Task UpdateSong(EditSongViewModel model)
        {
            var oldSong = _context.Songs.FirstOrDefault(x => x.Id == model.Id);
            oldSong.Title = model.Title;
            oldSong.FullTitle = model.FullTitle;
            oldSong.Text = model.Text;
            _context.Songs.Update(oldSong);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangeFavoriteStatus(int songId, string userId)
        {
           bool isFavoriteNow;
           var record = _context.FavoriteSongs.FirstOrDefault(x=>x.SongId == songId && x.UserId == userId);
           if(record == null)
           {
               _context.FavoriteSongs.Add(new FavoriteSong{UserId = userId, SongId = songId, AddedDate = DateTime.UtcNow});
               isFavoriteNow = true;
           }
           else
           {
               _context.FavoriteSongs.Remove(record);
               isFavoriteNow = false;
           }
           await _context.SaveChangesAsync();
           return isFavoriteNow;
        }

        public List<SongEntity> GetFavoriteSongs(int page, string userId)
        {
            int pageSize = SettingsConstants.SONGS_PAGE_SIZE;

            var songs = _context.FavoriteSongs.Include(x => x.Song)
                .Where(x => x.UserId == userId)
                .Where(x => !x.Song.IsDeleted && x.Song.PublishedAt != null)
                .OrderByDescending(x=>x.AddedDate)
                .Select(x => x.Song)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var song in songs)
            {
                song.Text = null;
            }
            return _mapper.Map<List<SongEntity>>(songs);
        }

        public List<SongEntity> GetLatestSongs(int page, bool mainPage)
        {
            int pageSize = mainPage ? 5 : SettingsConstants.SONGS_PAGE_SIZE;

            var songs = _context.Songs
                .Include(x => x.Artist)
                .Where(x => !x.IsDeleted && x.PublishedAt != null)
                .OrderByDescending(x => x.PublishedAt)
                .Skip((page-1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var song in songs)
            {
                song.Text = null;
            }
            return _mapper.Map<List<SongEntity>>(songs);
        }

        public List<TopSongsViewItem> GetTopSongs(int page, bool mainPage)
        {
            int pageSize = mainPage ? 5 : SettingsConstants.SONGS_PAGE_SIZE;
            return _context.TopSongs
                .OrderByDescending(x => x.TotalViews)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<SongEntity> GetUnpublishedSongs(int page, bool mainPage)
        {
            int pageSize = mainPage ? 5 : SettingsConstants.SONGS_PAGE_SIZE;
            var songs = _context.Songs.Where(x => !x.IsDeleted && x.PublishedAt == null)
                .OrderBy(x => x.CreatedAt) // oldest first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach(var song in songs)
            {
                song.Text = null;
            }

            return _mapper.Map<List<SongEntity>>(songs);
        }

        public void PublishSong(int id, string userId)
        {
            var song = _context.Songs.FirstOrDefault(x => x.Id == id);
            song.PublishedAt = DateTime.UtcNow;
            song.PublishedUserId = userId;
            _context.Update(song);
            _context.SaveChanges();
        }

        public void Delete(int songId, string userId)
        {
            var song = _context.Songs.FirstOrDefault(x => x.Id == songId);
            song.IsDeleted = true;
            _context.Update(song);
            _context.SaveChanges();
            _logger.LogInformation($"The user with ID {userId} has deleted the song with ID {songId}");
        }

        public string Transpose(int songId, int tone)
        {
            var song = _context.Songs.FirstOrDefault(x=>x.Id == songId);
            if(song == null)
            {
                return null;
            }

            var chordsRow = new List<string>
			{
				"C", "C#", "D", "D#",
				"E", "F", "F#", "G",
				"G#", "A", "A#", "B"
			};
            return formatLyrics(Regex.Replace(song.Text, @"<chord>(.*?)<\/chord>",
				(match) =>
				{
					var chord = new Chord(match.Groups[1].Value);
					return $"<chord>{getTransposedChord(chord)}</chord>";
				}));

			string getTransposedChord(Chord currentChord)
			{
				if(currentChord.Note == 'H') currentChord.Note = 'B';
				int notePos = chordsRow.IndexOf(currentChord.Note.ToString());
				if(notePos < 0)
				{
					throw new Exception($"Не удалось транспонировать аккорд [{currentChord.Value}]");
				}
				if(currentChord.IsDies) notePos++;
				else if(currentChord.IsBemol) notePos--;

                notePos += tone;
				
				notePos = notePos % chordsRow.Count;
                if(notePos < 0)
                {
                    notePos = chordsRow.Count + notePos;
                }

				StringBuilder newChord = new StringBuilder(chordsRow[notePos]);
				if(currentChord.IsMinor)
				{
					newChord.Append("m");
				}
				newChord.Append(currentChord.RestPart);
				return newChord.ToString();
			}
        }

        // ������� ��� ������� � ����������� html-����
        private string formatLyrics(string text)
        {
            string pattern = @"(^|\s|\(|\/?)([ABCDEFGH][#-]?(msus4|m6|m7|m9|maj7|maj9|maj|add9|m7b5|7b5|7#5|7b9|aug|dim|dim7|sus2|sus4|m|b|5|7|6|9|13)?)([^\w]|$)";
            pattern = @"<chord>(.*?)<\/chord>";
            string replaceString = "<span className=\"chord\">$1</span>";
            return Regex.Replace(text, pattern, replaceString);
        }
    }
}