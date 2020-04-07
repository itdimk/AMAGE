using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace AMAGE.Imaging.Adapters
{
    internal static class EncoderSelector
    {
        private static Dictionary<string, Type> encoders
            = new Dictionary<string, Type>()
        {
            { "bmp", typeof(BmpBitmapEncoder) },
            { "gif", typeof(GifBitmapEncoder) },
            { "png", typeof(PngBitmapEncoder) },
            { "jpg", typeof(JpegBitmapEncoder) },
            { "tiff", typeof(TiffBitmapEncoder) },
            { "jpeg", typeof(JpegBitmapEncoder) },
        };

        static internal BitmapEncoder GetEncoder(string format)
        {
            format = format.Replace(".", "");
            return (BitmapEncoder)encoders[format].GetConstructor(new Type[0])
                .Invoke(new object[0]);
        }
    }
}
