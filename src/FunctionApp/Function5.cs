using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Shared.Models;
using Twilio;

namespace FunctionApp
{
    [StorageAccount("my-storage-connection")]
    public static class Function5
    {
        [FunctionName("Function5")]
        [return: TwilioSms(AccountSidSetting = "twilio-account-sid",
                           AuthTokenSetting = "twilio-account-auth-token")]
        public static SMSMessage Run(
            [QueueTrigger("to-admin-notif")]         AnalysisReq request,
            TraceWriter                              log)
        {
            log.Info("(Fun5) Sending SMS...");

            var baseUrl = ConfigurationManager.AppSettings["base-url"];
            var from = ConfigurationManager.AppSettings["twilio-from-number"];
            var to = ConfigurationManager.AppSettings["twilio-to-number"];

            return new SMSMessage
            {
                From = from,
                To = to,
                Body = $@"Someone uploaded an non appropriated image to your site.
        The image url Id is {request.BlobRef},
        url is {baseUrl + request.ImageUrl}"

            };
        }
    }
}
