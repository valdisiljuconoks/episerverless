using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Shared.Models;

namespace FunctionApp
{
    public static class Function3
    {
        [FunctionName(nameof(Function3))]
        public static async Task<AsciiArtResult> Run(
            [ActivityTrigger]                                            AsciiArtRequest request,
                                                                         TraceWriter log)
        {
            log.Info($"({nameof(Function3)}) Starting to convert image to ASCII art...");

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["my-storage-connection"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(ConfigurationManager.AppSettings["input-container"]);
            var inBlob = blobContainer.GetBlockBlobReference($"{request.BlobRef}").GetByteArrayFromCloudBlockBlob();

            var convertedImage = ConvertImageToAscii(inBlob, request.Width);

            var blobOutputContainer = blobClient.GetContainerReference(ConfigurationManager.AppSettings["output-container"]);
            var outBlob = blobOutputContainer.GetBlockBlobReference($"{request.BlobRef}");

            await outBlob.UploadFromByteArrayAsync(convertedImage, 0, convertedImage.Length);

            var result = new AsciiArtResult(request.BlobRef,
                                            ConfigurationManager.AppSettings["output-container"],
                                            request.Description,
                                            request.Tags);

            log.Info($"({nameof(Function3)}) Finished converting image.");

            return result;
        }























        // Copyright: Code for ASCII convert used from http://www.c-sharpcorner.com/article/generating-ascii-art-from-an-image-using-C-Sharp/
        private static readonly string[] _AsciiChars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", "&nbsp;" };

        private static byte[] ConvertImageToAscii(Stream image, int width)
        {
            var bitmap = new Bitmap(image, true);
            bitmap = GetReSizedImage(bitmap, width);
            return Encoding.UTF8.GetBytes(ConvertToAscii(bitmap));
        }

        private static Bitmap GetReSizedImage(Image inputBitmap, int asciiWidth)
        {
            var asciiHeight = (int)Math.Ceiling((double)inputBitmap.Height * asciiWidth / inputBitmap.Width);
            var result = new Bitmap(asciiWidth, asciiHeight);
            using(var g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(inputBitmap, 0, 0, asciiWidth, asciiHeight);
                return result;
            }
        }

        private static string ConvertToAscii(Bitmap image)
        {
            var sb = new StringBuilder();
            var toggle = false;

            for(var h = 0; h < image.Height; h++)
            {
                for(var w = 0; w < image.Width; w++)
                {
                    var pixelColor = image.GetPixel(w, h);
                    var red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    var green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    var blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    var grayColor = Color.FromArgb(red, green, blue);

                    if(toggle)
                        continue;

                    var index = grayColor.R * 10 / 255;
                    sb.Append(_AsciiChars[index]);
                }

                if(!toggle)
                {
                    sb.Append("<BR>");
                    toggle = true;
                }
                else
                    toggle = false;
            }

            return sb.ToString();
        }
    }
}
