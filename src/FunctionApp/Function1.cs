using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
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

            var instanceId = await starter.StartNewAsync("ProcessingSequence", analysisRequest);
            var result = starter.CreateCheckStatusResponse(req, instanceId);

            return result;
        }
    }

    public static class ProcessingSequence
    {
        [FunctionName("ProcessingSequence")]
        [StorageAccount("my-storage-connection")]
        [return: Queue("done-images")]
        public static async Task<AsciiArtResult> Run(
            [OrchestrationTrigger]      DurableOrchestrationContext context,
                                        TraceWriter log)
        {
            var input = context.GetInput<AnalysisReq>();

            var visionResult = await context.CallActivityAsync<(string Description, string[] Tags)>("Function2", input);
            var asciiResult = await context.CallActivityAsync<AsciiArtResult>("Function3", input);

            var result = new AsciiArtResult(asciiResult.BlobRef,
                                            ConfigurationManager.AppSettings["output-container"],
                                            visionResult.Description,
                                            visionResult.Tags);

            log.Info("(ProcessingSequence) Finished processing the image.");

            return result;
        }
    }
}
