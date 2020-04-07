using AMAGE.Common.ComponentModel;
using AMAGE.Common.Extensions;
using AMAGE.Imaging.Properties;
using System.Threading.Tasks;
using System.Windows;
using AMAGE.Common.Imaging;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using AMAGE.Imaging.Tools.Tuners;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.HSLFilter))]
    public class HSLFiltering : BaseAsyncApplying, ITunerSupport, IImageSupport, IImageListSupport, ICustomAreaSupport, ICustomFramesSupport
    {
        private int hue;
        private float saturation;
        private float lightness;

        private readonly ISnapshotTuner tuner;

        [Browsable(false)]
        public ITuner Tuner => tuner;

        [Browsable(false)]
        public Int32Rect CustomArea { protected get; set; }

        [Browsable(false)]
        public IReadOnlyList<int> CustomFrames { protected get; set; }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Hue))]
        public int Hue
        {
            get { return hue; }
            set { hue = value.Clamp(0, 360); }
        }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Saturation))]
        public float Saturation
        {
            get { return saturation; }
            set { saturation = value.Clamp(-100.0F, 100.0F); }
        }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Lighting))]
        public float Lightness
        {
            get { return lightness; }
            set { lightness = value.Clamp(-100.0F, 100.0F); }
        }

        public HSLFiltering(ISnapshotTuner tuner)
        {
            this.tuner = tuner;
            tuner.TunableTool = this;
            tuner.SetSnapshotProperties(nameof(Hue), nameof(Saturation), nameof(Lightness));
        }

        public void LoadSettings(IImageList input)
        {
            tuner.SetSnapshotCount(1, input?.Count ?? 1);
        }

        public void LoadSettings(IImage input)
        {
            tuner.SetSnapshotCount(1, 1);
        }

        public override string ToString()
        {
            return Resources.HSLFilter;
        }

        public unsafe void Apply(IImage input, IImage output)
        {
            int* inputPtr = input.BeginPixelWorking(false);
            int* outputPtr = output.BeginPixelWorking(true);

            int width = Math.Min(input.Width, output.Width);
            int height = Math.Min(input.Height, output.Height);

            Apply(inputPtr, outputPtr, width, height, CustomArea, Hue, Saturation, Lightness);

            input.EndPixelWorking();
            output.EndPixelWorking();
        }

        public unsafe void Apply(IImageList input, IImageList output)
        {
            Int32Rect area = CustomArea;

            int[] hueValues = tuner.GetSnapshots<int>(nameof(Hue))
                .Interpolate(CustomFrames.Count);

            float[] lightnessValues = tuner.GetSnapshots<float>(nameof(Lightness))
                .Interpolate(CustomFrames.Count);

            float[] saturationValues = tuner.GetSnapshots<float>(nameof(Saturation))
                .Interpolate(CustomFrames.Count);

            Apply(input, output, (inputs, outputs, widths, heights) =>
            {
                for (int i = 0; i < CustomFrames.Count; ++i)
                {
                    int frame = CustomFrames[i];

                    Apply(inputs[frame], outputs[frame], widths[frame], heights[frame], area,
                        hueValues[i], saturationValues[i], lightnessValues[i]);
                }
            });
        }

        protected unsafe void Apply(int* input, int* output, int width, int height, Int32Rect area, int hue,
            float saturation, float lightness)
        {
            int startX = area.X;
            int startY = area.Y;
            int endX = area.X + area.Width;
            int endY = area.Y + area.Height;

            lightness = lightness / 100.0F;
            saturation = saturation / 100.0F;

            Parallel.For(0, height, (y) =>
            {
                for (int x = 0; x < width; ++x)
                {
                    if (x >= startX && x < endX && y >= startY && y < endY)
                    {
                        int argb = input[y * width + x];

                        byte r = (byte)(argb >> 16);
                        byte g = (byte)(argb >> 8);
                        byte b = (byte)(argb);

                        int h;
                        float s;
                        float l;

                        ColorEx.ToHSL(r, g, b, out h, out s, out l);

                        h = (h + hue) % 360;
                        s = (s + saturation).Clamp(0.0F, 1.0F);
                        l = (l + lightness).Clamp(0.0F, 1.0F);

                        ColorEx.ToRgb(h, s, l, out r, out g, out b);

                        output[y * width + x] = (argb & -0x1000000) | (r << 16) | (g << 8) | (b);
                    }
                    else
                        output[y * width + x] = input[y * width + x];
                }
            });
        }
    }
}
