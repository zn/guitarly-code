using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore;
using AutoMapper;
using Data;
using IdentityModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Models.DataModels;
using Models.EntityModels;
using Models.ViewModels;

namespace Api.Services
{
    public class UsersService
    {
        private readonly ILogger<UsersService> _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UsersService(ILogger<UsersService> logger, AppDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public UserEntity GetUser(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            return _mapper.Map<UserEntity>(user);
        }

        public async Task<string> GetTokenAsync(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SettingsConstants.VK_SECRET_KEY));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: SettingsConstants.JWT_ISSUER,
                audience: SettingsConstants.JWT_AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
