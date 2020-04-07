using System.IO;
using System.Windows.Media.Imaging;

namespace AMAGE.Common.Imaging
{
    public interface IImage
    {
        int Width { get; }
        int Height { get; }

        int PixelCount { get; }
        int AnimationDelay { get; set; }

        unsafe int* BeginPixelWorking(bool writable);
        void EndPixelWorking();

        BitmapSource ToBitmapSource();
        void FromBitmapSource(BitmapSource image);

        void ToFile(string fileName, string format, int qualityPercent = 100);
        void FromFile(string fileName);

        void ToStream(Stream output, string format, int qualityPercent = 100);
        void FromStream(Stream input);

        void CloneTo(IImage output);
        IImage Clone();
    }
}