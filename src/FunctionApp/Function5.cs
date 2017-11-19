using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Shared.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace FunctionApp
{
    [StorageAccount("my-storage-connection")]
    public static class Function5
    {
        [FunctionName("Function5")]
        public static void Run(
            [QueueTrigger("to-admin-notif")]         AnalysisReq request,
            TraceWriter                              log)
        {
            log.Info("(Fun5) Sending sms...");

            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
            var baseUrl = ConfigurationManager.AppSettings["base-url"];

            // Initialize the Twilio client
            TwilioClient.Init(accountSid, authToken);

            MessageResource.Create(
                                   @from: new PhoneNumber(ConfigurationManager.AppSettings["TwilioFromNumber"]),
                                   to: new PhoneNumber(ConfigurationManager.AppSettings["TwilioToNumber"]),
                                   body:
                                   $"Someone uploaded an non appropriated image to your site. The image url Id is {request.BlobRef} and the url is {baseUrl + request.ImageUrl}");

            log.Info("(Fun5) SNS sent.");
        }
    }
}
