namespace Shared.Models
{
    public class AsciiArtRequest
    {
        public string BlobRef { get; set; }

        public string[] Tags { get; set; }

        public string Description { get; set; }

        public int Width { get; set; }
    }
}
