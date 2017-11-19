namespace Shared.Models
{
    public class AsciiArtResult
    {
        public AsciiArtResult(string blobRef, string container, string description, string[] tags)
        {
            BlobRef = blobRef;
            Container = container;
            Description = description;
            Tags = tags;
        }

        public string BlobRef { get; }
        public string Container { get; }
        public string Description { get; }
        public string[] Tags { get; }
    }
}
