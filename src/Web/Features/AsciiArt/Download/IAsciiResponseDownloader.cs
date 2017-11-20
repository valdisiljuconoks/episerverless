using System.Text;

namespace Web1.Features.AsciiArt.Download
{
    public interface IAsciiResponseDownloader
    {
        void Download(StringBuilder log);
    }
}