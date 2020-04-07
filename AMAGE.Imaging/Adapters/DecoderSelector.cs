using System.IO;
using System.Windows.Media.Imaging;

namespace AMAGE.Imaging.Adapters
{
    internal static class DecoderSelector
    {
        static internal BitmapDecoder GetDecoder(Stream source, BitmapCreateOptions createOptions,
            BitmapCacheOption cacheOption)
        {
            return BitmapDecoder.Create(source, createOptions, cacheOption);
        }
    }
}
