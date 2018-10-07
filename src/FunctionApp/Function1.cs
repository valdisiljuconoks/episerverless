using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Shared.Models;

namespace FunctionApp
{
    [StorageAccount("my-storage-connection")]
    [ServiceBusAccount("my-servicebus-connection")]
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] ProcessingRequest request,
                                                                HttpRequestMessage req,
            [Blob("%input-container%/{FileId}")]                CloudBlockBlob outBlob,
            [OrchestrationClient]                               DurableOrchestrationClient starter,
                                                                TraceWriter log)
        {
            log.Info("(Fun1) Received image for processing...");

            await outBlob.UploadFromByteArrayAsync(request.Content, 0, request.Content.Length);
            var analysisRequest = new AnalysisReq
            {
                BlobRef = outBlob.Name,
                Width = request.Width,
                ImageUrl = request.ImageUrl
            };

            var instanceId = await starter.StartNewAsync(nameof(ProcessingSequence), analysisRequest);
            var result = starter.CreateCheckStatusResponse(req, instanceId);

            return result;
        }
    }
}
