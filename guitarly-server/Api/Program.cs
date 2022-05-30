using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.DataModels;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<AppDbContext>();
                try
                {
                    var config = services.GetRequiredService<IConfiguration>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var needSeed = config.GetSection("Data").GetValue<bool>("Seed");
                    if(needSeed)
                    {
                        logger.LogInformation("Initializing database...");
                        DatabaseInitializer.SeedData(context, userManager);
                        logger.LogInformation("The database is initialized. Please set Seed=false in appsettings.json");
                        return;
                    }
                    else
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => config.AddEnvironmentVariables())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
