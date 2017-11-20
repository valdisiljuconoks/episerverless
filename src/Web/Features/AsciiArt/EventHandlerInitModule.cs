using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using Web1.Business;
using Web1.Features.AsciiArt.Upload;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace Web1.Features.AsciiArt
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class EventHandlerInitModule : IInitializableModule
    {
        private IAsciiArtUploader _uploader;
        private UrlHelper _urlHelper;

        public void Initialize(InitializationEngine context)
        {
            var canon = ServiceLocator.Current.GetInstance<IContentEvents>();
            _uploader = ServiceLocator.Current.GetInstance<IAsciiArtUploader>();
            _urlHelper = ServiceLocator.Current.GetInstance<UrlHelper>();

            canon.CreatedContent += OnImageCreated;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var canon = ServiceLocator.Current.GetInstance<IContentEvents>();
            canon.CreatedContent -= OnImageCreated;
        }

        private void OnImageCreated(object sender, ContentEventArgs args)
        {
            if (!(args.Content is ImageData img))
                return;

            using (var stream = img.BinaryData.OpenRead())
            {
                var bytes = stream.ReadAllBytes();
                _uploader.Upload(img.ContentGuid.ToString(),
                                 bytes,
                                 _urlHelper.ContentUrl(img.ContentLink));
            }
        }
    }
}
