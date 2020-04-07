using System.Collections.Generic;
using AMAGE.Common.Imaging;
using AMAGE.Imaging.Tools.Tuners;
using System.ComponentModel;
using System.Threading.Tasks;
using System;
using AMAGE.Imaging.Properties;
using AMAGE.Common.Extensions;
using AMAGE.Common.ComponentModel;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.ExternalImageEditorOverlay))]
    public class ExternalOverlay : IImageListSupport, ITunerSupport, ICustomFramesSupport
    {
        public enum OverlayMode { Simple, DeltaChannel }

        private readonly IExternalOverlayTuner tuner;

        [Browsable(false)]
        public ITuner Tuner => tuner;

        [Browsable(false)]
        public IReadOnlyList<int> CustomFrames { protected get; set; }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Mode))]
        public OverlayMode Mode { get; set; }

        public ExternalOverlay(IExternalOverlayTuner tuner)
        {
            this.tuner = tuner;
            tuner.TunableTool = this;
            tuner.RefreshTunableTool();
        }

        public void LoadSettings(IImageList input)
        {
            if (input != null)
                tuner.Before = input[0].Clone();
            else
                tuner.Before = null;
        }

        public override string ToString()
        {
            return Resources.ExternalImageEditorOverlay;
        }

        public unsafe void Apply(IImageList input, IImageList output)
        {
            IImage before = tuner.Before;
            IImage after = tuner.After;

            if (before != null && after != null)
            {
                if (before.Width != after.Width || before.Height != after.Height)
                    throw new  Exception("Before size must be equal After size");

                int* beforePtr = before.BeginPixelWorking(false);
                int* afterPtr = after.BeginPixelWorking(false);

                foreach (int frame in CustomFrames)
                {
                    int* inputPtr = input[frame].BeginPixelWorking(false);
                    int* outputPtr = output[frame].BeginPixelWorking(true);

                    if (Mode == OverlayMode.Simple)
                        ApplySimple(beforePtr, afterPtr, inputPtr, outputPtr, before.PixelCount);
                    else if (Mode == OverlayMode.DeltaChannel)
                        ApplyDeltaChannel(beforePtr, afterPtr, inputPtr, outputPtr, before.PixelCount);

                    input[frame].EndPixelWorking();
                    output[frame].EndPixelWorking();
                }

                before.EndPixelWorking();
                after.EndPixelWorking();
            }
        }

        protected unsafe void ApplySimple(int* before, int* after, int* input, int* output, int pixelCount)
        {
            Parallel.For(0, pixelCount, (i) =>
            {
                if (before[i] != after[i])
                    output[i] = after[i];
                else
                    output[i] = input[i];
            });
        }

        protected unsafe void ApplyDeltaChannel(int* before, int* after, int* input, int* output, int pixelCount)
        {
            Parallel.For(0, pixelCount, (i) =>
            {
                if (before[i] != after[i])
                {
                    byte beforeA = (byte)(before[i] >> 24);
                    byte beforeR = (byte)(before[i] >> 16);
                    byte beforeG = (byte)(before[i] >> 8);
                    byte beforeB = (byte)(before[i]);

                    int deltaA = ((after[i] >> 24) & 0xFF) - beforeA;
                    int deltaR = ((after[i] >> 16) & 0xFF) - beforeR;
                    int deltaG = ((after[i] >> 8) & 0xFF) - beforeG;
                    int deltaB = (after[i] & 0xFF) - beforeB;

                    byte inputA = (byte)(input[i] >> 24);
                    byte inputR = (byte)(input[i] >> 16);
                    byte inputG = (byte)(input[i] >> 8);
                    byte inputB = (byte)(input[i]);

                    output[i] = ((inputA + deltaA).Clamp(0, 255) << 24) |
                                ((inputR + deltaR).Clamp(0, 255) << 16) |
                                ((inputG + deltaG).Clamp(0, 255) << 8) |
                                ((inputB + deltaB).Clamp(0, 255));
                }
                else
                    output[i] = input[i];
            });
        }
    }
}
