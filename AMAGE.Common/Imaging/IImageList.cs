using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace AMAGE.Common.Imaging
{
    public interface IImageList : IList<IImage>
    {
        void ToStream(Stream output, string format);
        void FromStream(Stream input);

        void ToFile(string fileName, string format);
        void FromFile(string fileName);

        BitmapSource[] ToBitmapSources();
        void FromBitmapSources(BitmapSource[] images);

        void CloneTo(IImageList output);
        IImageList Clone();
    }
}
