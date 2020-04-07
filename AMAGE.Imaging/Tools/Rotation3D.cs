using AMAGE.Common.ComponentModel;
using AMAGE.Common.Extensions;
using AMAGE.Common.Imaging;
using AMAGE.Imaging.Properties;
using AMAGE.Imaging.Tools.Tuners;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace AMAGE.Imaging.Tools
{
    [DisplayNameLocalizable(typeof(Resources), nameof(Resources.Rotation3D))]
    public class Rotation3D : IImageListSupport, ITunerSupport, ICustomFramesSupport
    {
        private readonly IRotation3DTuner tuner;

        [Browsable(false)]
        public ITuner Tuner => tuner;

        [Browsable(false)]
        public IReadOnlyList<int> CustomFrames { protected get; set; }

        [DisplayNameLocalizable(typeof(Resources), nameof(Resources.BackgroundColor))]
        public Color Background
        {
            get { return tuner.Background; }
            set { tuner.Background = value; }
        }

        public Rotation3D(IRotation3DTuner tuner)
        {
            this.tuner = tuner;
            tuner.TunableTool = this;
            tuner.SetSnapshotCount(1, int.MaxValue);
        }

        public void LoadSettings(IImageList input)
        {
            if (input != null)
                tuner.TargetImage = input[0].ToBitmapSource();
            else
                tuner.TargetImage = null;
        }

        public override string ToString()
        {
            return Resources.Rotation3D;
        }

        public void Apply(IImageList input, IImageList output)
        {
            IReadOnlyList<Matrix3D> snapshots = tuner.GetSnapshots().Interpolate(CustomFrames.Count);

            for (int i = 0; i < CustomFrames.Count; ++i)
            {
                int frame = CustomFrames[i];

                BitmapSource bitmap = input[frame].ToBitmapSource();
                bitmap = tuner.Render(bitmap, snapshots[i]);
                output[frame].FromBitmapSource(bitmap);
            }
        }
    }
}
