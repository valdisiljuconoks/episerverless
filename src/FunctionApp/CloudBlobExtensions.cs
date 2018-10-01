using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FunctionApp
{
    public static class CloudBlobExtensions
    {
        public static Stream GetByteArrayFromCloudBlockBlob(this CloudBlob blob)
        {
            var stream = new MemoryStream();
            blob.DownloadToStream(stream);
            stream.Position = 0;

            return stream;
        }
    }
}
