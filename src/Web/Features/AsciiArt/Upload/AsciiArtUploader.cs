using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Models;

namespace Web1.Features.AsciiArt.Upload
{
    internal class AsciiArtUploader : IAsciiArtUploader
    {
        private readonly IAsciiArtServiceSettingsProvider _settings;

        public AsciiArtUploader(IAsciiArtServiceSettingsProvider settings)
        {
            _settings = settings;
        }

        public void Upload(string fileId, byte[] bytes, string imageUrl)
        {
            AsyncHelper.RunSync(() => CallFunctionAsync(fileId, bytes, imageUrl));
        }

        private async Task<string> CallFunctionAsync(string contentReference,
            byte[] byteData,
            string imageUrl)
        {
            var req = new ProcessingRequest
            {
                FileId = contentReference,
                Content = byteData,
                Width = 150,
                ImageUrl = imageUrl
            };

            using (var content = new StringContent(JsonConvert.SerializeObject(req)))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await
                    Global.HttpClient.Value.PostAsync(
                        _settings.Settings.RequestFunctionUri,
                        content).ConfigureAwait(false);

                return await response.Content.ReadAsStringAsync()
                                             .ConfigureAwait(false);
            }
        }
    }
}