using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Shared.Models;
using Twilio;

namespace FunctionApp
{
    public static class Function5
    {
        [FunctionName(nameof(Function5))]
        [return: TwilioSms(AccountSidSetting = "twilio-account-sid",
                           AuthTokenSetting = "twilio-account-auth-token")]
        public static SMSMessage Run(
            [ActivityTrigger]         AnalysisReq request,
                                      TraceWriter log)
        {
            log.Info($"({nameof(Function5)}) Sending SMS...");

            var baseUrl = ConfigurationManager.AppSettings["base-url"];
            var from = ConfigurationManager.AppSettings["twilio-from-number"];
            var to = ConfigurationManager.AppSettings["twilio-to-number"];

            log.Info($"({nameof(Function5)}) Sent.");

            return new SMSMessage
            {
                From = from,
                To = to,
                Body = $@"Someone uploaded an non appropriated image to your site. Image url: {baseUrl + request.ImageUrl}"
            };
        }
    }
}
