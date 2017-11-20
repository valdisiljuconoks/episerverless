namespace Web1.Features.AsciiArt.Upload
{
    public interface IAsciiArtUploader
    {
        void Upload(string fileId, byte[] bytes, string imageUrl);
    }
}
