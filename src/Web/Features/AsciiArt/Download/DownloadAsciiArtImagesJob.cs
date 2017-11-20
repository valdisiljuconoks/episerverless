using System.Text;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace Web1.Features.AsciiArt.Download
{
    [ScheduledPlugIn(DisplayName = "Download AsciiArt Images")]
    public class DownloadAsciiArtImagesJob : ScheduledJobBase
    {
        private readonly IAsciiResponseDownloader _downloader;

        public DownloadAsciiArtImagesJob(IAsciiResponseDownloader downloader)
        {
            _downloader = downloader;
        }

        public override string Execute()
        {
            OnStatusChanged($"Starting execution of {GetType()}");

            var log = new StringBuilder();

            _downloader.Download(log);

            return log.ToString();
        }
    }
}