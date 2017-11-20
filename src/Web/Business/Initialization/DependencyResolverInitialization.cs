using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using Web1.Business.Rendering;
using Web1.Features.AsciiArt;
using Web1.Features.AsciiArt.Download;
using Web1.Features.AsciiArt.Upload;

namespace Web1.Business.Initialization
{
    [InitializableModule]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (o, e) =>
            {
                context.Services
                    .AddTransient<IContentRenderer, ErrorHandlingContentRenderer>()
                    .AddTransient<ContentAreaRenderer, AlloyContentAreaRenderer>()
                    .AddTransient<IAsciiArtUploader, AsciiArtUploader>()
                    .AddTransient<IAsciiResponseDownloader, CloudQueueAsciiResponseDownloader>()
                    .AddSingleton<IAsciiArtServiceSettingsProvider, AsciiArtServiceSettingsProvider>()
                    .AddTransient<IAsciiArtImageProcessor, AsciiArtImageProcessor>();
            };
        }

        public void Initialize(InitializationEngine context)
        {
            DependencyResolver.SetResolver(new ServiceLocatorDependencyResolver(context.Locate.Advanced));
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}
