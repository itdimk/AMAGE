using AMAGE.Services;
using LightInject;

namespace AMAGE.Presentation
{
    public interface IAppController
    {
        IEventController EventController { get; }
        IServiceContainer ServiceContainer { get; }

        void Run<TPresenter>()
            where TPresenter : IPresenter;
    }
}
