using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models.ViewModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Api.Services
{
    public class FilesService
    {
        private readonly ILogger<FilesService> _logger;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly string _imagesDirectoryPath;
        
        public FilesService(ILogger<FilesService> logger, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
            _imagesDirectoryPath = Path.Combine(_appEnvironment.WebRootPath, "images");
            if(!Directory.Exists(_imagesDirectoryPath))
            {
                Directory.CreateDirectory(_imagesDirectoryPath);
            }
        }

        public List<string> SaveArtistPicture(WritableArtistViewModel viewModel)
        {
            string artistDirectory = Path.Combine(_imagesDirectoryPath, "artists", viewModel.Slug);
            if(!Directory.Exists(artistDirectory))
            {
                Directory.CreateDirectory(artistDirectory);
            }
            var result = new List<string>(3);

            string fileNameFormat = viewModel.Slug + "{0}" + Path.GetExtension(viewModel.Image.FileName);
            using Stream originalFile = viewModel.Image.OpenReadStream();
            
            using (var fileStream = new FileStream(Path.Combine(artistDirectory, string.Format(fileNameFormat, "")), FileMode.Create))
            {
                originalFile.CopyTo(fileStream);
                result.Add(fileStream.Name.Replace(_appEnvironment.WebRootPath, ""));
            }
            originalFile.Seek(0, SeekOrigin.Begin);

            // 30x30
            string path30 = Path.Combine(artistDirectory, string.Format(fileNameFormat, "30"));
            cropAndResize(originalFile, true, 30, 30, path30);
            result.Add(path30.Replace(_appEnvironment.WebRootPath, ""));
            originalFile.Seek(0, SeekOrigin.Begin);

            string path100 = Path.Combine(artistDirectory, string.Format(fileNameFormat, "100"));
            cropAndResize(originalFile, true, 100, 100, path100);
            result.Add(path100.Replace(_appEnvironment.WebRootPath, ""));

            return result;
        }

        static void cropAndResize(Stream file, bool crop, int width, int height, string outputFullPath)
        {
            using Image image = Image.Load<Rgba32>(file);
            Size size = new Size(width, height);
            if (crop)
            {
                image.Mutate(i => i.Resize(new ResizeOptions
                {
                    Size = size,
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center
                }));
            }
            else
            {
                image.Mutate(i => i.Resize(new ResizeOptions
                {
                    Size = size,
                    Mode = ResizeMode.Min
                }));
            }

            string ext  = Path.GetExtension(outputFullPath);
            if(ext == ".jpg" || ext == ".jpeg")
            {
                image.Save(outputFullPath, new JpegEncoder { Quality = 100 });
            }
            else
            {
                image.Save(outputFullPath);
            }
        }
    }
}
