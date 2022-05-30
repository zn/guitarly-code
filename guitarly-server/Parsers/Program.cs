using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Parsers.JsonObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Parsers
{
    class Program
    {
        static int artistId;
        static string cookies = "_ga=GA1.2.50200577.1616828238; _ym_uid=1616828238868084665; _ym_d=1616828238; __gads=ID=29ddffce2cb22b75:T=1625326369:S=ALNI_Mb8pigNZsTmmEtQtloy4PLGdM63Bg; cto_bundle=LTcItF8lMkZjJTJGNjlLQnVjeVM0UXdyVzAxZTVpSGRTaGhuU0syTFRRZ3ZXa1Ntb0JVbjZMUFMlMkJBR0YxcXlNOUMwMXRIaVBENVJvV3NMNGlIQk1CMVRhd1JmdEtJWE9tQW1OOTJLS3ZQODMySVRQUkNYYTcyZ1NUT2pYMmdOSGp2SHZkYlU2bmQyNSUyQmpWanYwVWtKRFhPOE9XdlJmZyUzRCUzRA; notsy_ab_ym_1.0.7=A; PHPSESSID=3kfpir9kqpm4d1cpdb9idhrio4; _gid=GA1.2.1384765965.1625324940; _ym_isad=2; _ym_visorc=b; _gat=1";
        static HttpClient parserClient = new HttpClient();
        static HttpClient apiClient = new HttpClient();

        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();


            string authString = configuration["AuthString"];

            apiClient.BaseAddress = new Uri(configuration["ApiUrl"]);

            var form = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("QueryString",authString)
            });
            var response = apiClient.PostAsync("users/auth", form).Result;

            string content = response.Content.ReadAsStringAsync().Result;
            string token = JsonConvert.DeserializeObject<AuthResponse>(content).Token;
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            artistId = int.Parse(configuration["AuthorId"]);

            string artistUrl = configuration["ArtistUrl"];
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            var songsUrls = getSongsUrls(artistUrl).Result;

            var tasks = new List<Task>();
            foreach (var url in songsUrls)
            {
                tasks.Add(sendSong(url.Url));
                Console.WriteLine(url);
                Thread.Sleep(1300);
            }
            Task.WaitAll(tasks.ToArray());
        }


        static async Task sendSong(string songUrl)
        {
            var song = await getSongText(songUrl);
            if (song == null) return;

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FullTitle", song.FullTitle),
                new KeyValuePair<string, string>("Title", song.Title),
                new KeyValuePair<string, string>("Text", song.Lyrics),
                new KeyValuePair<string, string>("ArtistId", artistId.ToString())
            };

            var form = new FormUrlEncodedContent(formData);
            var result = await apiClient.PostAsync("/songs/new", form);
            Console.WriteLine($"{result.StatusCode} - {songUrl}");
        }

        static async Task<List<Composition>> getSongsUrls(string artistUrl)
        {
            string html = await parserClient.GetStringAsync(artistUrl);

            HtmlParser parser = new HtmlParser();
            var doc = parser.ParseDocument(html);
            var tableRows = doc.GetElementsByTagName("tbody").First().GetElementsByTagName("tr");
            return tableRows.Select(x => new Composition
                {
                    SongTitle = x.FirstElementChild.FirstElementChild.TextContent,
                    Url = "https:" + x.FirstElementChild.FirstElementChild.GetAttribute("href"),
                    Views = int.Parse(x.LastElementChild.TextContent.Trim(), NumberStyles.AllowThousands)
                })
                .OrderByDescending(x=>x.Views)
                .GroupBy(x=>x.SongTitle)
                .Select(x=>x.First())
                .ToList();
        }


        static async Task<Song> getSongText(string url)
        {
            string html = await parserClient.GetStringAsync(url);

            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var song = new Song();

            HtmlParser parser = new HtmlParser();
            var doc = parser.ParseDocument(html);

            var artistTitleNode = doc.GetElementsByTagName("span").First(x => x.GetAttribute("itemprop") == "byArtist");


            string songTitle = artistTitleNode.ParentElement.GetElementsByTagName("span").First(x => x.GetAttribute("itemprop") == "name").TextContent;

            song.FullTitle = $"{artistTitleNode.TextContent} - {songTitle}";
            song.Title = songTitle;

            var lyrics = doc.GetElementsByTagName("pre").First(x => x.GetAttribute("itemprop") == "chordsBlock");
            song.Lyrics = lyrics.InnerHtml;

            // regexp_replace(Text, '(.?)<b>(.?)<\/b>(.*?)', '\1\2\3', 'g')

            song.Lyrics = Regex.Replace(song.Lyrics, @"(.*?)<b>(.*?)<\/b>(.*?)", "$1$2$3");

            return song;
        }
    }
}
