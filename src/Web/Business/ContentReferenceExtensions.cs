using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Web1.Business
{
    public static class ContentReferenceExtensions
    {
        public static T GetContent<T>(this ContentReference target) where T : IContentData
        {
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var content = repo.Get<T>(target);

            return content;
        }
    }
}
