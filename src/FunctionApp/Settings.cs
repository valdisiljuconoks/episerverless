using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Shared.Models;

namespace FunctionApp
{
    public static class Settings
    {
        [FunctionName("Settings")]
        public static SettingsMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            TraceWriter log)
        {
            return new SettingsMessage(Environment.GetEnvironmentVariable("my-storage-connection"),
                                       Environment.GetEnvironmentVariable("done-queue"));
        }
    }
}
