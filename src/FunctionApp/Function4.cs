using System;
using System.Configuration;
using System.IO;
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
    public static class Function4
    {
        [FunctionName("Function4")]
        [return: Queue("to-admin-notif")]
        public static async Task<CloudQueueMessage> Run(
            [ServiceBusTrigger("mytopic", "to-eval", AccessRights.Manage)]   AnalysisReq request,
            [Blob("%input-container%/{BlobRef}", FileAccess.Read)]           Stream inBlob,
            TraceWriter                                                      log)
        {
            log.Info("(Fun4) Running image approval analysis...");

            try
            {
                var subscriptionKey = ConfigurationManager.AppSettings["cognitive-services-key"];
                var serviceUri = ConfigurationManager.AppSettings["cognitive-services-uri"];

                var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
                             {
                                 Endpoint = serviceUri
                             };

                var result = await client.AnalyzeImageInStreamAsync(inBlob,
                                                                    new[]
                                                                    {
                                                                        VisualFeatureTypes.Adult
                                                                    });

                if(result.Adult.IsAdultContent)
                {
                    log.Warning("(Fun4) Unappropriated content detected. Sending notification...");
                    return request.AsQueueItem();
                }

                log.Info("(Fun4) Image approved (no sensitive content).");
            }
            catch (Exception e)
            {
                log.Info("Error on Running image approval..." + e.Message);
            }

            return null;
        }
    }
}
