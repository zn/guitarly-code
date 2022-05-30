using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Api.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;

namespace Api.Controllers
{
    [ApiController]
    [Route("artists")]
    public class ArtistsController : ControllerBase
    {
        private readonly ArtistsService _artistsService;
        public ArtistsController(ArtistsService artistsService)
        {
            _artistsService = artistsService;
        }
        
        /// <summary>
        /// Возвращает информацию об исполнителе и список его песен
        /// </summary>
        /// <param name="id">ID артиста</param>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ViewArtist(int id)
        {
            string userId = User.Claims.First(x => x.Type == JwtClaimTypes.Subject).Value;
            var artist = _artistsService.GetById(id, userId);
            if (artist != null)
            {
                return Ok(artist);
            }
            return NotFound();
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        [Authorize(Policy = "Moder")]
        [HttpPost("add")]
        public IActionResult AddArtist([FromForm]WritableArtistViewModel model)
        {
            if(!ModelState.IsValid || model.Image.Length > 5 * 1024 * 1024) // 5 Mb
            {
                return BadRequest(model);
            }
            
            var artistId = _artistsService.AddArtist(model);
            return Ok(artistId);
        }

        
        //[HttpPost("favorite")]
        //public async Task<IActionResult> ChangeFavoriteStatus([FromForm]int artistId, [FromForm]int userId)
        //{
        //    if(artistId == 0 || userId == 0)
        //    {
        //        ModelState.AddModelError("", "Указаны некорректные id исполнителя и/или пользователя");
        //        return BadRequest(ModelState);
        //    }
        //    bool isFavoriteNow = await _artistsService.ChangeFavoriteStatus(artistId, userId);
        //    return Ok(isFavoriteNow);
        //}


        /// <summary>
        /// Возвращает список самых популярных исполнителей
        /// </summary>
        /// <param name="page">Номер страницы (на одной странице 10 исполнителей)</param>
        /// <returns></returns>
        [HttpGet("top")]
        public IActionResult GetTopArtists(int page = 1)
        {
            if (page < 1)
            {
                return BadRequest();
            }

            return Ok(_artistsService.GetTopArtists(page));
        }
    }
}
