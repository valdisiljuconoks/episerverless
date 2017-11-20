using Microsoft.WindowsAzure.Storage;
using Shared.Models;

namespace Web1.Features.AsciiArt.Download
{
    public interface IAsciiArtImageProcessor
    {
        void SaveAsciiArt(CloudStorageAccount account, AsciiArtResult result);
    }
}