using System;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Services
{
    public static class AllServices
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
            services.AddScoped<ArtistsService>();
            services.AddScoped<FilesService>();
            services.AddScoped<SongsService>();
            services.AddScoped<HomeService>();
            services.AddScoped<UsersService>();

            return services;
        }
    }
}
