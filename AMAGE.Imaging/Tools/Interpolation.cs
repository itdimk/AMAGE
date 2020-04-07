using AMAGE.Common.Imaging;
using System.Linq;
using AMAGE.Common.Extensions;
using AMAGE.Common.ComponentModel;
using AMAGE.Imaging.Properties;
using AMAGE.Imaging.Tools.Tuners;
using System.ComponentModel;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.FrameInterpolation))]
    public class Interpolation : IImageListSupport, ITunerSupport
    {
        private int targetFrameCount = 1;

        [Browsable(false)]
        public ITuner Tuner { get; }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.TargetFrameCount))]
        public int TargetFrameCount
        {
            get { return targetFrameCount; }
            set { targetFrameCount = value.Clamp(1, int.MaxValue); }
        }

        public Interpolation(ITuner tuner)
        {
            this.Tuner = tuner;
            tuner.TunableTool = this;
        }

        public virtual void LoadSettings(IImageList input)
        {
            TargetFrameCount = input?.Count ?? 1;
            Tuner.RefreshTunableTool();
        }

        public override string ToString()
        {
            return Resources.FrameInterpolation;
        }

        public unsafe void Apply(IImageList input, IImageList output)
        {
            int[] indices = Enumerable.Range(0, input.Count)
                 .Interpolate(TargetFrameCount);

            IImageList frames = ImageList.Create();

            foreach (int index in indices)
                frames.Add(input[index]);

            frames.CloneTo(output);
        }
    }
}
