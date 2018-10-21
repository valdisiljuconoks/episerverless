using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Shared.Models;

namespace FunctionApp
{
    public static class ProcessingSequence
    {
        [FunctionName(nameof(ProcessingSequence))]
        [StorageAccount("my-storage-connection")]
        [return: Queue("done-images")]
        public static async Task<AsciiArtResult> Run(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            TraceWriter log)
        {
            var input = context.GetInput<AnalysisReq>();

            var visionResult = await context.CallActivityAsync<(string Description, string[] Tags)>(nameof(Function2), input);
            var asciiResult = await context.CallActivityAsync<AsciiArtResult>(nameof(Function3), input);

            var adultContentResult = await context.CallActivityAsync<bool>(nameof(Function4), input);
            if(adultContentResult)
                await context.CallActivityAsync<TwilioSmsAttribute>(nameof(Function5), input);

            var result = new AsciiArtResult(asciiResult.BlobRef,
                                            ConfigurationManager.AppSettings["output-container"],
                                            visionResult.Description,
                                            visionResult.Tags);

            log.Info($"({nameof(ProcessingSequence)}) Finished processing the image.");

            return result;
        }
    }
}
