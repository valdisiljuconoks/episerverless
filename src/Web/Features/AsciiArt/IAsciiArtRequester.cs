namespace Web1.Features.AsciiArt
{
    public interface IAsciiArtRequester
    {
        void Upload(string fileId, byte[] bytes, string imageUrl);
    }
}
