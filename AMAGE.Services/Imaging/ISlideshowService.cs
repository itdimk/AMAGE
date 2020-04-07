using AMAGE.Common.Imaging;
using System;

namespace AMAGE.Services.Imaging
{
    public interface ISlideshowService
    {
        IRepository<IImageList> Repository { set; }

        void StartSlideshow(string imageKey, Action<IImage> callback, int repeats);
        void StopSlideshow(string imageKey);
        bool IsRunning(string imageKey);
    }
}
