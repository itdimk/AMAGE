using AMAGE.Common.Imaging;
using AMAGE.Imaging.Adapters;

namespace AMAGE.Imaging
{
    public static class ImageList
    {
        public static IImageList Create()
        {
            return new ImageListAdapter();
        }
    }
}
