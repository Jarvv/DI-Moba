using System;
using Core.Events;
using VContainer.Unity;

namespace Structures
{
    public class StructureLifecycle : IStartable, IDisposable
    {
        private readonly IEventBus _eventBus;
        private IDisposable _subscription;

        public StructureLifecycle(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Start()
        {
            _subscription = _eventBus.Subscribe<StructureDestroyedEvent>(OnStructureDestroyed);
        }

        private void OnStructureDestroyed(StructureDestroyedEvent e)
        {
            e.Structure.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
