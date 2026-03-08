using UnityEngine;
using System;

namespace Core.Events
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent eventData);
        IDisposable Subscribe<TEvent>(Action<TEvent> handler);
        void Unsubscribe<TEvent>(Action<TEvent> handler);
    }
}