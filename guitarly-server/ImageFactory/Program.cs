using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace ImageFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Set path for original image in the first argument");
                return;
            }

            string originalImageUrl = args[0];

            Stream originalFile = File.OpenRead(originalImageUrl);

            var fileInfo = new FileInfo(originalImageUrl);
            string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            // 30x30
            string resultPath = Path.Combine(fileInfo.DirectoryName, fileName + "30" + fileInfo.Extension);
            cropAndResize(originalFile, true, 30, 30, resultPath);
            originalFile.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("30x30 is ready");

            // 100x100
            resultPath = Path.Combine(fileInfo.DirectoryName, fileName + "100" + fileInfo.Extension);
            cropAndResize(originalFile, true, 100, 100, resultPath);
            originalFile.Seek(0, SeekOrigin.Begin);
            Console.WriteLine("100x100 is ready");
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
