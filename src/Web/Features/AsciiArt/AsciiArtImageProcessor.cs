using System;
using System.IO;
using System.Text;
using EPiServer;
using EPiServer.DataAccess;
using EPiServer.Security;
using Microsoft.WindowsAzure.Storage;
using Shared.Models;
using Web1.Business;
using Web1.Models.Media;

namespace Web1.Features.AsciiArt
{
    class AsciiArtImageProcessor : IAsciiArtImageProcessor
    {
        private readonly IContentRepository _repository;

        public AsciiArtImageProcessor(IContentRepository repository)
        {
            _repository = repository;
        }

        public void SaveAsciiArt(CloudStorageAccount account, AsciiArtResult result)
        {
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(result.Container);
            var asciiBlob = container.GetBlobReference(result.BlobRef);

            var image = _repository.Get<ImageFile>(Guid.Parse(result.BlobRef));
            var writable = image.MakeWritable<ImageFile>();

            using (var stream = new MemoryStream())
            {
                asciiBlob.DownloadToStream(stream);
                var asciiArt = Encoding.UTF8.GetString(stream.ToArray());

                writable.AsciiArt = asciiArt;
                writable.Description = result.Description;
                writable.Tags = string.Join(",", result.Tags);

                _repository.Save(writable, SaveAction.Publish, AccessLevel.NoAccess);
            }
        }
    }
}
