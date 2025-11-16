using System;
using System.Collections.Generic;

namespace Game.Events
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : IEvent;
        void Unsubscribe<T>(Action<T> handler) where T : IEvent;
        void Publish<T>(T evt) where T : IEvent;
    }

    public class EventBus : IEventBus
    {
        readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (!_handlers.TryGetValue(typeof(T), out var list))
                list = _handlers[typeof(T)] = new List<Delegate>();

            list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            if (_handlers.TryGetValue(typeof(T), out var list))
                list.Remove(handler);
        }

        public void Publish<T>(T evt) where T : IEvent
        {
            if (_handlers.TryGetValue(typeof(T), out var list))
                foreach (var l in list)
                    (l as Action<T>)?.Invoke(evt);
        }
    }
}
