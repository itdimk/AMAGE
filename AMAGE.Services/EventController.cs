using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AMAGE.Services
{
    public class EventController : IEventController
    {
        public event EventHandler<object> SubscriptionsChanged;

        private readonly Dictionary<EventInfo, List<Delegate>> events
            = new Dictionary<EventInfo, List<Delegate>>();

        private readonly Dictionary<EventInfo, List<object>> objects
            = new Dictionary<EventInfo, List<object>>();

        private readonly Dictionary<EventInfo, List<Func<bool>>> conditions
            = new Dictionary<EventInfo, List<Func<bool>>>();

        private readonly List<EventInfo> disabledEvents
            = new List<EventInfo>();

        public IEventController Subscribe(object target, string eventName, params EventHandler[] handlers)
        {
            Subscribe(target, eventName, (Delegate[])handlers);

            Unsubscribe(target, eventName, (Delegate)(EventHandler)RefreshConditions);
            Subscribe(target, eventName, (Delegate)(EventHandler)RefreshConditions);

            return this;
        }

        public IEventController Subscribe<T>(object target, string eventName, params EventHandler<T>[] handlers)
        {
            Subscribe(target, eventName, (Delegate[])handlers);

            Unsubscribe(target, eventName, (Delegate)(EventHandler<T>)RefreshConditions);
            Subscribe(target, eventName, (Delegate)(EventHandler<T>)RefreshConditions);

            return this;
        }

        public IEventController AddCondition(object target, string eventName, Func<bool> condition)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);

            if (!conditions.ContainsKey(eventInfo))
                conditions.Add(eventInfo, new List<Func<bool>>());

            conditions[eventInfo].Add(condition);

            RefreshConditions(this, EventArgs.Empty);
            return this;
        }

        public IEventController RemoveCondition(object target, string eventName, Func<bool> condition)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);

            if (conditions.ContainsKey(eventInfo))
                conditions.Remove(eventInfo);

            RefreshConditions(this, EventArgs.Empty);
            return this;
        }

        public IEventController Unsubscribe(object target, string eventName, params EventHandler[] handlers)
        {
            Unsubscribe(target, eventName, (Delegate[])handlers);
            return this;
        }

        public IEventController Unsubscribe<T>(object target, string eventName, params EventHandler<T>[] handlers)
        {
            Unsubscribe(target, eventName, (Delegate[])handlers);
            return this;
        }

        public void UnsubscribeAll(object target, string eventName)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);

            if(events.ContainsKey(eventInfo))
                Unsubscribe(target, eventName, events[eventInfo].ToArray());
        }

        private void Subscribe(object target, string eventName, params Delegate[] handlers)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);

            if (!events.ContainsKey(eventInfo))
                events.Add(eventInfo, new List<Delegate>());

            if (!objects.ContainsKey(eventInfo))
                objects.Add(eventInfo, new List<object>());

            if (!objects[eventInfo].Contains(target))
                objects[eventInfo].Add(target);

            foreach (Delegate handler in handlers)
            {
                events[eventInfo].Add(handler);
                eventInfo.AddEventHandler(target, handler);
            }

            RefreshConditions(this, EventArgs.Empty);
            SubscriptionsChanged?.Invoke(this, target);
        }

        private void Unsubscribe(object target, string eventName, params Delegate[] handlers)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);

            foreach (Delegate handler in handlers)
            {
                events[eventInfo].Remove(handler);
                eventInfo.RemoveEventHandler(target, handler);
            }

            RefreshConditions(this, EventArgs.Empty);
            SubscriptionsChanged?.Invoke(this, target);
        }

        private void Disable(object target, string eventName)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);

            foreach (Delegate handler in events[eventInfo])
                eventInfo.RemoveEventHandler(target, handler);

            if (!disabledEvents.Contains(eventInfo))
                disabledEvents.Add(eventInfo);

            SubscriptionsChanged?.Invoke(this, target);
        }

        private void Enable(object target, string eventName)
        {
            Disable(target, eventName);

            EventInfo eventInfo = target.GetType().GetEvent(eventName);

            foreach (Delegate handler in events[eventInfo])
                eventInfo.AddEventHandler(target, handler);

            if (disabledEvents.Contains(eventInfo))
                disabledEvents.Remove(eventInfo);

            SubscriptionsChanged?.Invoke(this, target);
        }

        private void RefreshConditions<T>(object sender, T e)
        {
            foreach (KeyValuePair<EventInfo, List<Func<bool>>> item in conditions)
            {
                bool condition = item.Value.All(c => c.Invoke());

                foreach (object obj in objects[item.Key])
                {
                    if (condition)
                        Enable(obj, item.Key.Name);
                    else
                        Disable(obj, item.Key.Name);
                }
            }
        }
    }
}
