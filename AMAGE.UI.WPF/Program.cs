using System;
using AMAGE.Presentation;
using LightInject;
using AMAGE.Services;
using AMAGE.Presentation.Presenters;
using System.Windows;
using AMAGE.Imaging.Tools;
using AMAGE.Presentation.View.ImageEditor;
using AMAGE.Common.Imaging;
using AMAGE.Services.Imaging;
using AMAGE.UI.WPF.Tuners;
using System.Threading;
using System.Globalization;

namespace AMAGE.UI.WPF
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application app = new Application();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");

            IServiceContainer serviceContainer = new ServiceContainer();
            IEventController eventController = new EventController();
            IAppController appController = new AppController(serviceContainer, eventController);

            IMessageService messageSvc = new MessageService();
            ILogService logSvc = new LogService();

            IImageSupport[] imageTools = new IImageSupport[]
            {

            };

            IImageListSupport[] imageListTools = new IImageListSupport[]
            {
                new HSLFiltering(new SnapshotTuner()),
                new ColorFiltering(new SnapshotTuner()),
                new Resize(new Tuner()),
                new ExternalOverlay(new ExternalOverlayTuner()),
                new Rotation3D(new Rotation3DTuner()),
                new SpeedControl(new SnapshotTuner()),
                new Interpolation(new Tuner()),
                new Convolution(new ConvolutionTuner())
            };

            appController.ServiceContainer
                .Register<IImageEditor, ImageEditor.ImageEditor>()
                .Register<IRepository<IImageList>, RepositoryService<IImageList>>()
                .Register<ISlideshowService, SlideshowService>();

            appController.ServiceContainer
                .RegisterInstance(appController)
                .RegisterInstance(imageTools)
                .RegisterInstance(imageListTools)
                .RegisterInstance(messageSvc)
                .RegisterInstance(logSvc);

            appController.Run<ImageEditorPresenter>();

            app.Run();
        }
    }
}
