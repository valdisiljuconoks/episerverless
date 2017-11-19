using System.IO;

namespace Web1.Business
{
    public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream is MemoryStream stream1)
                return stream1.ToArray();

            using (var stream2 = new MemoryStream())
            {
                stream.CopyTo(stream2);
                return stream2.ToArray();
            }
        }
    }
}
