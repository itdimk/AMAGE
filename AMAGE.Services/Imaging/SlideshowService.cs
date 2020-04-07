using System;
using AMAGE.Common.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace AMAGE.Services.Imaging
{
    public sealed class SlideshowService : ISlideshowService
    {
        private readonly Dictionary<string, Task> Tasks
            = new Dictionary<string, Task>();

        private readonly Dictionary<string, CancellationTokenSource> Cancels
            = new Dictionary<string, CancellationTokenSource>();

        public IRepository<IImageList> Repository { private get; set; }

        public bool IsRunning(string imageKey)
        {
            return Tasks.ContainsKey(imageKey) && Tasks[imageKey].Status == TaskStatus.Running;
        }

        public void StartSlideshow(string imageKey, Action<IImage> callback, int repeats)
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            if (!IsRunning(imageKey))
            {
                Cancels[imageKey] = new CancellationTokenSource();

                Tasks[imageKey] = Task.Run(() =>
                {
                    do
                    {
                        IImageList imageList = null;
                        for (int i = 0; Repository.TryGetValue(imageKey, out imageList) && i < imageList.Count; ++i)
                        {
                            if (Cancels[imageKey].Token.IsCancellationRequested)
                                return;

                            IImage frame = imageList.ElementAtOrDefault(i);
                            Task callbackTask = new Task(() => callback?.Invoke(frame));
                            callbackTask.RunSynchronously(scheduler);
                            callbackTask.Wait();

                            Thread.Sleep(frame.AnimationDelay);
                        }
                    }
                    while (Repository.ContainsKey(imageKey) && --repeats != 0);
                }, Cancels[imageKey].Token);
            }
        }

        public void StopSlideshow(string imageKey)
        {
            Cancels[imageKey].Cancel();
        }
    }
}
