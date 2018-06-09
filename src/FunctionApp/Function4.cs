//using System;
//using System.Configuration;
//using System.IO;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Microsoft.ServiceBus.Messaging;
//using Microsoft.WindowsAzure.Storage.Queue;
//using Newtonsoft.Json;
//using Shared.Models;

//namespace FunctionApp
//{
//    [StorageAccount("my-storage-connection")]
//    [ServiceBusAccount("my-servicebus-connection")]
//    public static class Function4
//    {
//        [FunctionName("Function4")]
//        [return: Queue("to-admin-notif")]
//        public static async Task<CloudQueueMessage> Run(
//            [ServiceBusTrigger("mytopic", "to-eval", AccessRights.Manage)]   AnalysisReq request,
//            [Blob("%input-container%/{BlobRef}", FileAccess.Read)]           Stream inBlob,
//            TraceWriter                                                      log)
//        {
//            log.Info("(Fun4) Running image approval analysis...");

//            try
//            {
//                var subscriptionKey =
//                    ConfigurationManager.AppSettings["cognitive-services-approval-key"];

//                var client = new HttpClient();

//                // Request headers
//                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

//                // Request parameters
//                var uri = ConfigurationManager.AppSettings["cognitive-services-approval-uri"];

//                using (var ms = new MemoryStream())
//                {
//                    inBlob.CopyTo(ms);
//                    var byteArray = ms.ToArray();
//                    Task<string> result;

//                    using (var content = new ByteArrayContent(byteArray))
//                    {
//                        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
//                        var response = await client.PostAsync(uri, content);
//                        result = response.Content.ReadAsStringAsync();
//                    }

//                    if(result.Result != null)
//                    {
//                        var resultAsObject = JsonConvert.DeserializeObject<ContentModeratorResult>(result.Result);

//                        if(resultAsObject.IsImageAdultClassified)
//                        {
//                            log.Warning("(Fun4) Inappropriate content detected. Sending notification...");
//                            return request.AsQueueItem();
//                        }

//                        log.Warning("(Fun4) Image approved (no sensitive content).");
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                log.Info("Error on Running image approval..." + e.Message);
//            }

//            return null;
//        }
//    }
//}
