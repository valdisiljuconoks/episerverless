using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Shared.Models;

namespace FunctionApp
{
    public static class Function2
    {
        [FunctionName("Function2")]
        public static async Task<(string Description, string[] Tags)> Run(
            [ActivityTrigger]                                      AnalysisReq request,
                                                                   TraceWriter log)
        {
            log.Info("(Fun2) Running image analysis...");
            var subscriptionKey = ConfigurationManager.AppSettings["cognitive-services-key"];
            var serviceUri = ConfigurationManager.AppSettings["cognitive-services-uri"];

            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
                         {
                             Endpoint = serviceUri
                         };

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["my-storage-connection"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(ConfigurationManager.AppSettings["input-container"]);
            var inBlob = blobContainer.GetBlockBlobReference($"{request.BlobRef}").GetByteArrayFromCloudBlockBlob();

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

            log.Info("(Fun2) Finished image analysis.");

            return (string.Join(",", result.Description.Captions.Select(c => c.Text)),
                    result.Tags.Select(t => t.Name).ToArray());
        }
    }
}
