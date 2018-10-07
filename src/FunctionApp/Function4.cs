using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Shared.Models;

namespace FunctionApp
{
    public static class Function4
    {
        [FunctionName(nameof(Function4))]
        public static async Task<bool> Run(
            [ActivityTrigger]              AnalysisReq request,
                                           TraceWriter log)
        {
            log.Info($"({nameof(Function4)}) Running image approval analysis...");
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
                                                                    VisualFeatureTypes.Adult,
                                                                });

            log.Info($"({nameof(Function4)}) Finished image approval analysis.");

            return result.Adult.IsAdultContent;
        }
    }
}
