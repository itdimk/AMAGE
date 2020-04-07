using System;

namespace AMAGE.Services
{
    public interface IEventController
    {
        event EventHandler<object> SubscriptionsChanged;

        IEventController Subscribe(object target, string eventName, params EventHandler[] handlers);
        IEventController Subscribe<T>(object target, string eventName, params EventHandler<T>[] handlers);

        IEventController Unsubscribe(object target, string eventName, params EventHandler[] handlers);
        IEventController Unsubscribe<T>(object target, string eventName, params EventHandler<T>[] handlers);

        IEventController AddCondition(object target, string eventName, Func<bool> condition);
        IEventController RemoveCondition(object target, string eventName, Func<bool> condition);

        void UnsubscribeAll(object target, string eventName);
    }
}
