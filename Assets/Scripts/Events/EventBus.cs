using System;
using System.Collections.Generic;

namespace Game.Events
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T evt);
    }

    public class EventBus : IEventBus
    {
        readonly Dictionary<Type, Delegate> _handlers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if (_handlers.TryGetValue(t, out var del)) _handlers[t] = Delegate.Combine(del, handler);
            else _handlers[t] = handler;
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if (_handlers.TryGetValue(t, out var del))
            {
                var newDel = Delegate.Remove(del, handler);
                if (newDel == null) _handlers.Remove(t);
                else _handlers[t] = newDel;
            }
        }

        public void Publish<T>(T evt)
        {
            if (_handlers.TryGetValue(typeof(T), out var del))
            {
                var action = del as Action<T>;
                action?.Invoke(evt);
            }
        }
    }
}
