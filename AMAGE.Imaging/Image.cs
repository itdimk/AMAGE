using AMAGE.Common.Imaging;
using AMAGE.Imaging.Adapters;

namespace AMAGE.Imaging
{
    public static class Image
    {
        public static IImage Create()
        {
            return new ImageAdapter();
        }
    }
}
