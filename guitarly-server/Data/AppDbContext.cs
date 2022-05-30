using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.DataModels;

namespace Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<ArtistAlternativeName> ArtistAlternativeNames { get; set; }
        public DbSet<TopArtistsViewItem> TopArtists { get; set; }
        public DbSet<TopSongsViewItem> TopSongs { get; set; }
        public DbSet<FavoriteSong> FavoriteSongs{get;set;}

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         
            modelBuilder.Entity<FavoriteSong>(entity => entity.HasKey(e => new { e.SongId, e.UserId }));
            modelBuilder.Entity<TopArtistsViewItem>().ToView("top_artists");
            modelBuilder.Entity<TopSongsViewItem>().ToView("top_songs");

            // Преобразование наименований Таблиц, Полей таблиц, Ключей, Индексов, Схем к виду snake_case
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetName(index.GetName().ToSnakeCase());
                }

                entity.SetSchema(entity.GetSchema().ToSnakeCase());
            }
        }
    }


    public static class StringExtensions
    {
        /// <summary>
        /// Преобразует строку к стилю snake_case, при котором несколько слов разделяются 
        /// символом подчеркивания '_', и не имеют пробелов в записи, каждое слово пишется с маленькой буквы
        /// </summary>
        /// <param name="input">Преобразуемая строка</param>
        /// <returns>Строка в формате snake_case</returns>
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            else
            {
                var startUnderscores = Regex.Match(input, @"^_+");

                return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
            }
        }
    }
}
