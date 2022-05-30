using System;
using System.Threading;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using IdentityModel;

namespace Api.Controllers
{
    [ApiController]
    [Route("home")]
    public class HomeController : ControllerBase
    {
        private readonly HomeService _homeService;
        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }
        
        
        /// <summary>
        /// Возвращает данные для главной страницы (последние песни, топ артистов, топ песен)
        /// </summary>
        [HttpGet("feed")]
        public IActionResult Index()
        {
            //Thread.Sleep(3000);
            string userRole = User.Claims.First(x => x.Type == JwtClaimTypes.Role).Value;
            return Ok(_homeService.GetPageModel(userRole));
        }

        /// <summary>
        /// Поиск по базе данных
        /// </summary>
        /// <param name="query">Строка поискового запроса</param>
        /// <returns>Список артистов и песен, подходящих под поисковый запрос</returns>
        [HttpGet("search")]
        public IActionResult Search([FromQuery(Name="q")]string query)
        {
            return Ok(_homeService.Search(query));
        }
    }
}
