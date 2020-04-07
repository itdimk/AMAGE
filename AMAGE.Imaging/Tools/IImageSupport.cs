using AMAGE.Common.Imaging;

namespace AMAGE.Imaging.Tools
{
    public interface IImageSupport
    {
        void Apply(IImage input, IImage output);
        void LoadSettings(IImage input);
    }
}
