using LightInject;
using AMAGE.Services;

namespace AMAGE.Presentation
{
    public class AppController : IAppController
    {
        public IServiceContainer ServiceContainer { get; }
        public IEventController EventController { get; }

        public AppController(IServiceContainer serviceContainer, IEventController eventController)
        {
            ServiceContainer = serviceContainer;
            EventController = eventController;
        }
        public void Run<TPresenter>() where TPresenter : IPresenter
        {
            if (!ServiceContainer.CanGetInstance(typeof(TPresenter), string.Empty))
                ServiceContainer.Register<TPresenter>();

            TPresenter presenter = ServiceContainer.GetInstance<TPresenter>();
            presenter.Run();
        }
    }
}
