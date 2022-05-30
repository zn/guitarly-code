using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.Services;
using ApplicationCore;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Models.DataModels;
using System.Net;
using Models.ViewModels;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UsersService usersService, UserManager<User> userManager, ILogger<UsersController> logger)
        {
            _usersService = usersService;
            _userManager = userManager;
            _logger = logger;
        }

        // ����� �������� ������ ��� ����� ��� �������� ����������.
        // ����� ������� �� ���, �� ��������� ��������, ����� ������ ����� ��������� � ���� ��������.
        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromForm]AuthViewModel viewModel)
        {
            _logger.LogInformation($"Get auth request data: [{viewModel.QueryString}]");
            var query = QueryHelpers.ParseQuery(viewModel.QueryString);
            
            if (!isParamsValid(query))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(query["vk_user_id"]);
            if(user == null)
            {
                user = new User
                {
                    Id = query["vk_user_id"],
                    UserName = query["vk_user_id"],
                    RegisterDate = DateTime.UtcNow,
                    LastLoginTime = DateTime.UtcNow,
                    ReferedFrom = query["vk_ref"],
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Sex = viewModel.Sex,
                    Photo100 = viewModel.Photo100,
                    Photo200 = viewModel.Photo200,
                    PhotoMaxOrig = viewModel.PhotoMaxOrig
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
                await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, RolesConstants.USER));
            }
            else if(!await _userManager.IsInRoleAsync(user, RolesConstants.ADMIN))
            {
                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;
                user.Sex = viewModel.Sex;
                user.Photo100 = viewModel.Photo100;
                user.Photo200 = viewModel.Photo200;
                user.PhotoMaxOrig = viewModel.PhotoMaxOrig;
                user.LastLoginTime = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            var token = await _usersService.GetTokenAsync(user);

            return Ok(new { token });
        }

        // ��� ������� ����������, ��� �������� ��������� ������. ���� ��������� �� ��������� �� ��
        // https://vk.com/dev/vk_apps_docs3
        private bool isParamsValid(Dictionary<string, StringValues> @params)
        {
            string sign = @params.First(x => x.Key == "sign").Value;
            // ��������� ���������, ������������ � �������� vk_
            var vkKeys = @params.Where(x => x.Key.StartsWith("vk_")).OrderBy(x=>x.Key).ToList();
            string payload = string.Join('&', vkKeys.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SettingsConstants.VK_SECRET_KEY));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            string base64 = Convert.ToBase64String(hash);

            // ����� �����-�� ����������
            base64 = base64.TrimEnd('=')
                           .Replace('+', '-')
                           .Replace('/', '_');

            return base64 == sign;
        }
    }
}
