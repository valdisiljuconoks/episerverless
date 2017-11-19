using System;
using System.Configuration;
using Newtonsoft.Json;
using Shared.Models;
using Web1.Business;

namespace Web1.Features.AsciiArt
{
    class AsciiArtServiceSettingsProvider : IAsciiArtServiceSettingsProvider
    {
        private readonly Lazy<SettingsMessage> _cachedInstance = new Lazy<SettingsMessage>(GetSettings);

        public SettingsMessage Settings => _cachedInstance.Value;

        private static SettingsMessage GetSettings()
        {
            var response = AsyncHelper.RunSync(() => Global.HttpClient.Value.GetAsync(ConfigurationManager.AppSettings["func:SettingsUri"]));
            var result = JsonConvert.DeserializeObject<SettingsMessage>(AsyncHelper.RunSync(() => response.Content.ReadAsStringAsync()));

            // TODO: inject settings from web.config differently
            result.RequestFunctionUri = ConfigurationManager.AppSettings["func:RequestAsciiUri"];
            return result;
        }
    }
}
