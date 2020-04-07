using System.Windows.Media;
using System.Windows;
using System.Threading.Tasks;
using AMAGE.Common.Extensions;
using AMAGE.Common.ComponentModel;
using AMAGE.Imaging.Properties;
using AMAGE.Common.Imaging;
using System.ComponentModel;
using System;
using AMAGE.Imaging.Tools.Tuners;
using System.Collections.Generic;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.ColorFilter))]
    public class ColorFiltering : BaseAsyncApplying, ITunerSupport, IImageSupport, IImageListSupport, ICustomAreaSupport, ICustomFramesSupport
    {
        private readonly ISnapshotTuner tuner;

        [Browsable(false)]
        public ITuner Tuner => tuner;

        [Browsable(false)]
        public Int32Rect CustomArea { protected get; set; }

        [Browsable(false)]
        public IReadOnlyList<int> CustomFrames { protected get; set; }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.TargetColor))]
        public Color TargetColor { get; set; } = Color.FromArgb(128, 0, 0, 0);

        public ColorFiltering(ISnapshotTuner tuner)
        {
            this.tuner = tuner;
            tuner.TunableTool = this;
            tuner.SetSnapshotProperties(nameof(TargetColor));
        }

        public void LoadSettings(IImageList input)
        {
            tuner.SetSnapshotCount(1, input?.Count ?? 1);
        }

        public void LoadSettings(IImage input)
        {
            tuner.SetSnapshotCount(1, 1);
        }

        public unsafe void Apply(IImage input, IImage output)
        {
            int* inputPtr = input.BeginPixelWorking(false);
            int* outputPtr = output.BeginPixelWorking(true);

            int width = Math.Min(input.Width, output.Width);
            int height = Math.Min(input.Height, output.Height);

            Apply(inputPtr, outputPtr, width, height, CustomArea, TargetColor);

            input.EndPixelWorking();
            output.EndPixelWorking();
        }

        public unsafe void Apply(IImageList input, IImageList output)
        {
            Int32Rect area = CustomArea;

            Color[] colorValues = tuner.GetSnapshots<Color>(nameof(TargetColor))
                .Interpolate(CustomFrames.Count);

            Apply(input, output, (inputs, outputs, widths, heights) =>
            {
                for (int i = 0; i < CustomFrames.Count; ++i)
                {
                    int frame = CustomFrames[i];

                    Apply(inputs[frame], outputs[frame], widths[frame], heights[frame], area,
                        colorValues[i]);
                }
            });
        }

        public override string ToString()
        {
            return Resources.ColorFilter;
        }

        protected unsafe void Apply(int* input, int* output, int width, int height, Int32Rect area,
            Color targetColor)
        {
            int startX = area.X;
            int startY = area.Y;
            int endX = area.X + area.Width;
            int endY = area.Y + area.Height;

            byte targetR = targetColor.R;
            byte targetG = targetColor.G;
            byte targetB = targetColor.B;

            float alphaPercent = targetColor.A / 255.0F;

            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; ++x)
                {
                    if (x >= startX && x < endX && y >= startY && y < endY)
                    {
                        int argb = input[y * width + x];

                        int r = (argb >> 16) & 0xFF;
                        int g = (argb >> 8) & 0xFF;
                        int b = (argb) & 0xFF;

                        r = (r + (int)((targetR - r) * alphaPercent)).Clamp(0, 255);
                        g = (g + (int)((targetG - g) * alphaPercent)).Clamp(0, 255);
                        b = (b + (int)((targetB - b) * alphaPercent)).Clamp(0, 255);

                        output[y * width + x] = (argb & -0x1000000) | (r << 16) | (g << 8) | (b);
                    }
                    else
                        output[y * width + x] = input[y * width + x];
                }
            });
        }
    }
}
