using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace AMAGE.Imaging.Tools.Tuners
{
    public interface IRotation3DTuner : ITuner
    {
        BitmapSource TargetImage { set; }
        Color Background { get; set; }

        IReadOnlyList<Matrix3D> GetSnapshots();
        BitmapSource Render(BitmapSource image, Matrix3D snapshot);
        void SetSnapshotCount(int minSnapshots, int maxSnapshots);
    }
}
