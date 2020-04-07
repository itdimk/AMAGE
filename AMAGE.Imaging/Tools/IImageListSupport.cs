using AMAGE.Common.Imaging;

namespace AMAGE.Imaging.Tools
{
    public interface IImageListSupport
    {
        void Apply(IImageList input, IImageList output);
        void LoadSettings(IImageList input);
    }
}
