using Shared.Models;

namespace Web1.Features.AsciiArt
{
    public interface IAsciiArtServiceSettingsProvider
    {
        SettingsMessage Settings { get; }
    }
}
