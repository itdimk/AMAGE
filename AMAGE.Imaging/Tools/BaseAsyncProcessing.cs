using AMAGE.Common.Imaging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AMAGE.Imaging.Tools
{
    public abstract class BaseAsyncApplying : IAsyncApplyingSupport 
    {
        public unsafe delegate void ImageListProcessingDelegate(int*[] inputs, int*[] outputs,
            int[] widths, int[] heights);

        protected Task AsyncApplyingTask;

        public event EventHandler AsyncApplyingCompleted;

        [Browsable(false)]
        public bool AllowMultipleTasks { get; set; }

        [Browsable(false)]
        public bool ApplyAsync { get; set; }

        protected unsafe void Apply(IImageList input, IImageList output, ImageListProcessingDelegate processing)
        {
            if (AllowMultipleTasks || (AsyncApplyingTask?.IsCompleted != false))
            {
                TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

                int frameCount = input.Count;

                int*[] inputs = new int*[frameCount];
                int*[] outputs = new int*[frameCount];
                int[] widths = new int[frameCount];
                int[] heights = new int[frameCount];

                for (int i = 0; i < input.Count; ++i)
                {
                    inputs[i] = input[i].BeginPixelWorking(false);
                    outputs[i] = output[i].BeginPixelWorking(true);
                    widths[i] = input[i].Width;
                    heights[i] = input[i].Height;
                }

                Action mainTask = () =>
                {
                    processing?.Invoke(inputs, outputs, widths, heights);
                };

                Action<Task> continueTask = (t) =>
                {

                    for (int i = 0; i < input.Count; ++i)
                    {
                        input[i].EndPixelWorking();
                        output[i].EndPixelWorking();
                    }

                    AsyncApplyingCompleted?.Invoke(this, EventArgs.Empty);
                };

                if (ApplyAsync)
                {
                    AsyncApplyingTask = Task.Run(mainTask).ContinueWith(continueTask, scheduler);
                }
                else
                {
                    mainTask();

                    for (int i = 0; i < input.Count; ++i)
                    {
                        input[i].EndPixelWorking();
                        output[i].EndPixelWorking();
                    }
                }
            }
        }
    }
}
