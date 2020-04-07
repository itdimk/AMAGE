using AMAGE.Presentation.View;
using AMAGE.Services;

namespace AMAGE.Presentation.Presenters
{
    public class BasePresenter<TView> : IPresenter
      where TView : IView
    {
        protected TView View;
        protected IAppController AppController;
        protected ILogService LogService;
        protected IMessageService MessageService;

        public BasePresenter(IAppController appController, TView view,
            ILogService logService, IMessageService messageService)
        {          
            AppController = appController;
            View = view;
            LogService = logService;
            MessageService = messageService;

            AppController.EventController.SubscriptionsChanged += EventController_SubscriptionsChanged;
        }

        private void EventController_SubscriptionsChanged(object sender, object e)
        {
            (e as IEnabledBySubscrSwitcher)?.SwitchEnabledBySubscription();
        }

        public virtual void Run()
        {
            View.Show();
        }
    }
}
