using System;
using System.Security.Claims;
using ApplicationCore;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.DataModels;

namespace Data
{
    public static class DatabaseInitializer
    {
        public static void SeedData(AppDbContext dbContext, UserManager<User> userManager)
        {
            dbContext.Database.ExecuteSqlRaw(@"
                CREATE OR REPLACE VIEW top_artists AS
                SELECT a.id, a.title, SUM(s.views_number) total_views,
                       a.picture30, a.picture100, a.picture_original
                FROM artists a
                JOIN songs s ON a.id = s.artist_id AND NOT s.is_deleted AND s.published_at IS NOT NULL
                GROUP BY a.id
                ORDER BY total_views DESC");

            dbContext.Database.ExecuteSqlRaw(@"
                CREATE OR REPLACE VIEW top_songs AS
                SELECT s.id, s.title, s.full_title, SUM(s.views_number) total_views, a.id as artist_id
                FROM songs s
                JOIN artists a on s.artist_id = a.id
                WHERE NOT s.is_deleted AND s.published_at IS NOT NULL
                GROUP BY s.id, a.id
                ORDER BY total_views DESC");
                
            if (!dbContext.Users.AnyAsync().Result)
            {
                var admin = new User
                {
                    Id = "139588023",
                    UserName = "139588023", // ↑ same
                    RegisterDate = DateTime.UtcNow,
                    NotificationsEnabled = true
                };
                var result = userManager.CreateAsync(admin).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }

                var roleClaim = new Claim(JwtClaimTypes.Role, RolesConstants.ADMIN);
                result = userManager.AddClaimAsync(admin, roleClaim).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }
        }
    }
}
