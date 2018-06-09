using System;
using System.Collections.Generic;
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
            [OrchestrationClient]                               DurableOrchestrationClientBase starter,
                                                                TraceWriter log)

            //[ServiceBus("mytopic", AccessRights.Manage)]        out BrokeredMessage topicMessage)
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

            //result.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(10));

            return result;

            //topicMessage = new BrokeredMessage(analysisRequest);
            //return new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StringContent(outBlob.Name)
            //};
        }
    }


    public static class ProcessingSequence
    {
        [FunctionName("ProcessingSequence")]
        public static async Task<List<string>> Run([OrchestrationTrigger] DurableOrchestrationContextBase context)
        {

            var input = context.GetInput<AnalysisReq>();

            var outputs = new List<string>();

            outputs.Add(await context.CallActivityAsync<string>("E1_SayHello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("E1_SayHello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("E1_SayHello", "London"));

            return outputs;
        }

        [FunctionName("E1_SayHello")]
        public static string SayHello([ActivityTrigger] string name)
        {
            return $"Hello {name}!";
        }
    }
}
