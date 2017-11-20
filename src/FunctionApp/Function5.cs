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
            log.Info("(Fun5) Sending SMS...");

            var accountSid = ConfigurationManager.AppSettings["twilio-account-sid"];
            var authToken = ConfigurationManager.AppSettings["twilio-account-auth-token"];
            var baseUrl = ConfigurationManager.AppSettings["base-url"];
            var from = ConfigurationManager.AppSettings["twilio-from-number"];
            var to = ConfigurationManager.AppSettings["twilio-to-number"];

            TwilioClient.Init(accountSid, authToken);

            MessageResource.Create(from: new PhoneNumber(from),
                                   to: new PhoneNumber(to),
                                   body:
        $@"Someone uploaded an non appropriated image to your site.
        The image url Id is {request.BlobRef},
        url is {baseUrl + request.ImageUrl}");

            log.Info("(Fun5) SMS sent.");
        }
    }
}
