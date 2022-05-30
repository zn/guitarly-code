using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Services;
using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;

namespace Api.Controllers
{
    [ApiController]
    [Route("songs")]
    public class SongsController : ControllerBase
    {
        private readonly SongsService _songsService;
        private readonly ArtistsService _artistsService;
        private readonly IMapper _mapper;
        public SongsController(SongsService songsService, ArtistsService artistsService, IMapper mapper)
        {
            _songsService = songsService;
            _artistsService = artistsService;
            _mapper = mapper;
        }

        /// <summary>
        /// Добавление песни
        /// </summary>
        /// <returns></returns>
        [HttpGet("add")]
        public IActionResult AddSong()
        {
            var model = _artistsService.GetAddSongModel();
            return Ok(model);
        }

        /// <summary>
        /// Добавление песни
        /// </summary>
        /// <param name="model">Данные песни</param>
        /// <returns></returns>
        [Authorize(Policy = "Moder")]
        [HttpPost("add")]
        public async Task<IActionResult> AddSong([FromForm] WritableSongViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var songId = await _songsService.AddSong(model);
            return Ok(songId);
        }

        /// <summary>
        /// Редактирование песни
        /// </summary>
        /// <param name="id">ID песни</param>
        /// <returns></returns>
        [Authorize(Policy = "Moder")]
        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var viewModel = _songsService.GetEditSongViewModel(id);
            return Ok(viewModel);
        }

        /// <summary>
        /// Редактирование песни
        /// </summary>
        /// <param name="id">ID песни</param>
        /// <param name="model">Обновленные данные песни</param>
        /// <returns></returns>
        [Authorize(Policy = "Moder")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm] EditSongViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.Id = id;

            await _songsService.UpdateSong(model);
            return Ok(true);
        }

        /// <summary>
        /// Получить песню по ID
        /// </summary>
        /// <param name="id">ID песни</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public IActionResult ViewSong(int id)
        {
            string userId = User.Claims.First(x => x.Type == JwtClaimTypes.Subject).Value;
            var song = _songsService.GetById(id, userId);
            if (song != null)
            {
                return Ok(song);
            }
            return NotFound();
        }

        /// <summary>
        /// Публикация песни
        /// </summary>
        /// <param name="id">ID песни</param>
        /// <returns></returns>
        [Authorize(Policy = "Moder")]
        [HttpPost("publish/{id}")]
        public IActionResult Publish(int id)
        {
            string userId = User.Claims.First(x => x.Type == JwtClaimTypes.Subject).Value;
            _songsService.PublishSong(id, userId);
            return Ok(true);
        }

        /// <summary>
        /// Получить последние добавленные песни
        /// </summary>
        /// <param name="page">Номер страницы</param>
        /// <param name="mainPage">Если true, то возвращает 5 песен, иначе - 20</param>
        /// <returns></returns>
        [HttpGet("new")]
        public IActionResult GetLatestSongs(int page = 1, bool mainPage = false)
        {
            if(page < 1)
            {
                return BadRequest();
            }

            return Ok(_songsService.GetLatestSongs(page, mainPage));
        }

        /// <summary>
        /// Получить самые популярные песни
        /// </summary>
        /// <param name="page">Номер страницы</param>
        /// <param name="mainPage">Если true, то возвращает 5 песен, иначе - 20</param>
        /// <returns></returns>
        [HttpGet("top")]
        public IActionResult GetTopSongs(int page = 1, bool mainPage = false)
        {
            if(page < 1)
            {
                return BadRequest();
            }

            return Ok(_songsService.GetTopSongs(page, mainPage));
        }

        /// <summary>
        /// Получить список неопубликованных песен
        /// </summary>
        /// <param name="page">ID песни</param>
        /// <param name="mainPage">Если true, то возвращает 5 песен, иначе - 20</param>
        /// <returns></returns>
        [Authorize(Policy = "Moder")]
        [HttpGet("unpublished")]
        public IActionResult GetUnpublishedSongs(int page = 1, bool mainPage = false)
        {
            if(page < 1)
            {
                return BadRequest();
            }

            return Ok(_songsService.GetUnpublishedSongs(page, mainPage));
        }

        /// <summary>
        /// Добавляет/удаляет песню из песенника
        /// </summary>
        /// <param name="songId">ID песни</param>
        /// <returns>true - песня добавлена в песенник, false - песня удалена из песенника</returns>
        [HttpPost("favorite")]
        public async Task<IActionResult> ChangeFavoriteStatus(int songId)
        {
            string userId = User.Claims.First(x => x.Type == JwtClaimTypes.Subject).Value;
            return Ok(await _songsService.ChangeFavoriteStatus(songId, userId));
        }

        /// <summary>
        /// Возвращает песни для песенника
        /// </summary>
        /// <param name="page">Номер страницы (на одной странице 20 песен)</param>
        /// <returns></returns>
        [HttpGet("favorites")]
        public IActionResult GetFavoriteSongs(int page = 1)
        {
            string userId = User.Claims.First(x => x.Type == JwtClaimTypes.Subject).Value;
            return Ok(_songsService.GetFavoriteSongs(page, userId));
        }

        /// <summary>
        /// Удаление песни
        /// </summary>
        /// <param name="songId">ID песни</param>
        /// <returns></returns>
        [Authorize(Policy = "Moder")]
        [HttpDelete("delete/{songId}")]
        public IActionResult Delete(int songId)
        {
            string userId = User.Claims.First(x => x.Type == JwtClaimTypes.Subject).Value;
            _songsService.Delete(songId, userId);
            return Ok(true);
        }

        /// <summary>
        /// Транспонирование аккордов
        /// </summary>
        /// <param name="songId">ID песни</param>
        /// <param name="tone">Тон</param>
        /// <returns>Новый текст песни</returns>
        [HttpPost("transpose")]
        public IActionResult Transpose([FromForm]int songId, [FromForm]int tone)
        {
            if(tone > 11 || tone < -11)
            {
                return BadRequest();
            }
            var newText = _songsService.Transpose(songId, tone);
            return Ok(newText);
        }
    }
}
