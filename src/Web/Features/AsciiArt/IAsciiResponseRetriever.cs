using System.Text;

namespace Web1.Features.AsciiArt {
    public interface IAsciiResponseRetriever
    {
        void Pump(StringBuilder log);
    }
}