using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Queue;
using Shared.Models;

namespace FunctionApp
{
    [StorageAccount("my-storage-connection")]
    [ServiceBusAccount("my-servicebus-connection")]
    public static class Function2
    {
        [FunctionName("Function2")]
        [return: Queue("to-ascii-conversion")]
        public static async Task<CloudQueueMessage> Run(
            [ServiceBusTrigger("mytopic", "to-ascii", AccessRights.Manage)]        AnalysisReq request,
            [Blob("%input-container%/{BlobRef}", FileAccess.Read)]                 Stream inBlob,
                                                                                   TraceWriter log)
        {
            log.Info("(Fun2) Running image analysis...");

            var subscriptionKey = ConfigurationManager.AppSettings["cognitive-services-key"];
            var serviceUri = ConfigurationManager.AppSettings["cognitive-services-uri"];
            var client = new VisionServiceClient(subscriptionKey, serviceUri);

            var result = await client.AnalyzeImageAsync(inBlob,
                                                        new[]
                                                        {
                                                            VisualFeature.Categories,
                                                            VisualFeature.Color,
                                                            VisualFeature.Description,
                                                            VisualFeature.Faces,
                                                            VisualFeature.ImageType,
                                                            VisualFeature.Tags
                                                        });

            var asciiArtRequest = new AsciiArtRequest
                                  {
                                      BlobRef = request.BlobRef,
                                      Width = request.Width,
                                      Description = string.Join(",", result.Description.Captions.Select(c => c.Text)),
                                      Tags = result.Tags.Select(t => t.Name).ToArray()
                                  };

            log.Info("(Fun2) Finished image analysis.");

            return asciiArtRequest.AsQueueItem();
        }
    }
}
