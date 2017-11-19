using EPiServer.Core;

namespace Web1.Business
{
    public static class ContentDataExtensions
    {
        public static T MakeWritable<T>(this ContentData target) where T : class
        {
            return target.CreateWritableClone() as T;
        }
    }
}
