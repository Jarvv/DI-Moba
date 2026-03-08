using System;
using System.Collections.Generic;

namespace Core.Events
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Publish<TEvent>(TEvent eventData)
        {
            var type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out List<Delegate> list)) return;

            foreach (var handler in list.ToArray())
                ((Action<TEvent>)handler).Invoke(eventData);
        }

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            var type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out List<Delegate> list))
                _handlers[type] = list = new List<Delegate>();

            list.Add(handler);
            return new Subscription(() => Unsubscribe(handler));
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            var type = typeof(TEvent);
            if (_handlers.TryGetValue(type, out List<Delegate> list))
                list.Remove(handler);
        }

        private sealed class Subscription : IDisposable
        {
            private Action _onDispose;

            public Subscription(Action onDispose) => _onDispose = onDispose;

            public void Dispose()
            {
                _onDispose?.Invoke();
                _onDispose = null;
            }
        }
    }
}
