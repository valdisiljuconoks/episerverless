using System.Text;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace Web1.Features.AsciiArt.Retrieval
{
    [ScheduledPlugIn(DisplayName = "Download AsciiArt Images")]
    public class DownloadAsciiArtImages : ScheduledJobBase
    {
        private readonly IAsciiResponseRetriever _retriever;

        public DownloadAsciiArtImages(IAsciiResponseRetriever retriever)
        {
            _retriever = retriever;
        }

        public override string Execute()
        {
            OnStatusChanged($"Starting execution of {GetType()}");

            var log = new StringBuilder();

            _retriever.Pump(log);

            return log.ToString();
        }
    }
}
