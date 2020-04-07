using AMAGE.Common.Imaging;
using AMAGE.Imaging.Tools.Tuners;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using AMAGE.Imaging.Properties;
using AMAGE.Common.ComponentModel;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.ConvolutionFilter))]
    public class Convolution : ITunerSupport, IImageSupport, IImageListSupport,
        ICustomFramesSupport
    {
        private IConvolutionTuner tuner;

        public ITuner Tuner => tuner;

        public IReadOnlyList<int> CustomFrames { private get; set; }

        public Convolution(IConvolutionTuner tuner)
        {
            this.tuner = tuner;
            tuner.TunableTool = this;
        }

        public void LoadSettings(IImageList input)
        {

        }

        public void LoadSettings(IImage input)
        {

        }

        public void Apply(IImageList input, IImageList output)
        {
            foreach(int frame in CustomFrames)
                Apply(input[frame], output[frame]);
        }

        public void Apply(IImage input, IImage output)
        {
            WriteableBitmap writeable;
            BitmapSource inputBitmap = input.ToBitmapSource();

            writeable = BitmapFactory.ConvertToPbgra32Format(inputBitmap);

            output.FromBitmapSource(writeable.Convolute(tuner.Kernel, tuner.KernelFactorSum,
                tuner.KernelOffsetSum));
        }

        public override string ToString()
        {
            return Resources.ConvolutionFilter;
        }
    }
}

