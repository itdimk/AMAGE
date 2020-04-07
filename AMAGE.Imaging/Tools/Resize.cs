using AMAGE.Common.Imaging;
using AMAGE.Imaging.Tools.Tuners;
using System.ComponentModel;
using AMAGE.Imaging.Properties;
using AMAGE.Common.ComponentModel;
using AMAGE.Common.Extensions;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Resize))]
    public class Resize : ITunerSupport, IImageSupport, IImageListSupport
    {
        private int width = 1;
        private int height = 1;

        [Browsable(false)]
        public ITuner Tuner { get; }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Width))]
        public int Width
        {
            get { return width; }
            set { width = value.Clamp(1, 8000); }
        }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Height))]
        public int Height
        {
            get { return height; }
            set { height = value.Clamp(1, 6000); }
        }

        public Resize(ITuner tuner)
        {
            this.Tuner = tuner;
            tuner.TunableTool = this;
        }

        public void LoadSettings(IImageList input)
        {
            LoadSettings(input?[0]);
        }

        public void LoadSettings(IImage input)
        {
            Width = input != null ? input.Width : 0;
            Height = input != null ? input.Height : 0;

            Tuner.RefreshTunableTool();
        }

        public override string ToString()
        {
            return Resources.Resize;
        }

        public void Apply(IImageList input, IImageList output)
        {
            for (int i = 0; i < input.Count; ++i)
                Apply(input[i], output[i]);
        }

        public void Apply(IImage input, IImage output)
        {
            WriteableBitmap writeable;
            BitmapSource inputBitmap = input.ToBitmapSource();

            if (inputBitmap.Format == PixelFormats.Pbgra32)
                writeable = new WriteableBitmap(inputBitmap);
            else
                writeable = BitmapFactory.ConvertToPbgra32Format(inputBitmap);

            output.FromBitmapSource(writeable.Resize(Width, Height,
                WriteableBitmapExtensions.Interpolation.Bilinear));
        }
    }
}
