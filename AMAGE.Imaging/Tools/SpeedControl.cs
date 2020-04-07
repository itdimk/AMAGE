using AMAGE.Common.Imaging;
using AMAGE.Common.ComponentModel;
using AMAGE.Imaging.Properties;
using AMAGE.Imaging.Tools.Tuners;
using System.ComponentModel;
using System.Collections.Generic;
using AMAGE.Common.Extensions;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.SpeedControl))]
    public class SpeedControl : ITunerSupport, IImageSupport, IImageListSupport, ICustomFramesSupport
    {
        private readonly ISnapshotTuner tuner;

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.TargetDelay))]
        public int TargetDelay { get; set; }

        [Browsable(false)]
        public ITuner Tuner => tuner;

        [Browsable(false)]
        public IReadOnlyList<int> CustomFrames { protected get; set; }

        public SpeedControl(ISnapshotTuner tuner)
        {
            this.tuner = tuner;
            tuner.TunableTool = this;
            tuner.SetSnapshotProperties(nameof(TargetDelay));
        }

        public void LoadSettings(IImage input)
        {
            tuner.SetSnapshotCount(1, 1);
            TargetDelay = input.AnimationDelay;
            tuner.RefreshTunableTool();
        }

        public void LoadSettings(IImageList input)
        {
            if (input != null && input.Count > 0)
            {
                tuner.SetSnapshotCount(1, input?.Count ?? 1);
                TargetDelay = input[0].AnimationDelay;
                tuner.RefreshTunableTool();
            }
        }

        public override string ToString()
        {
            return Resources.SpeedControl;
        }

        public void Apply(IImage input, IImage output)
        {
            output.AnimationDelay = TargetDelay;
        }

        public void Apply(IImageList input, IImageList output)
        {
            int[] delayValues = tuner.GetSnapshots<int>(nameof(TargetDelay))
                         .Interpolate(CustomFrames.Count);

            for (int i = 0; i < CustomFrames.Count; ++i)
            {
                int frame = CustomFrames[i];

                output[frame].AnimationDelay = delayValues[i];
            }
        }
    }
}
