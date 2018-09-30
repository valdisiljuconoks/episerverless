using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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

            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
                                 {
                                     Endpoint = serviceUri
                                 };

            //var client = new VisionServiceClient(subscriptionKey, serviceUri);

            var result = await client.AnalyzeImageInStreamAsync(inBlob,
                                                                new[]
                                                                {
                                                                    VisualFeatureTypes.Categories,
                                                                    VisualFeatureTypes.Color,
                                                                    VisualFeatureTypes.Description,
                                                                    VisualFeatureTypes.Faces,
                                                                    VisualFeatureTypes.ImageType,
                                                                    VisualFeatureTypes.Tags
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
