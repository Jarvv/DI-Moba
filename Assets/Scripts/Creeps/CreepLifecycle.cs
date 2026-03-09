using System;
using Core.Events;
using Core.Pooling;
using VContainer.Unity;

namespace Creeps
{
    public class CreepLifecycle : IStartable, IDisposable
    {
        private readonly GameObjectPool _pool;
        private readonly IEventBus _eventBus;
        private IDisposable _subscription;

        public CreepLifecycle(GameObjectPool pool, IEventBus eventBus)
        {
            _pool = pool;
            _eventBus = eventBus;
        }

        public void Start()
        {
            _subscription = _eventBus.Subscribe<CreepDiedEvent>(OnCreepDied);
        }

        private void OnCreepDied(CreepDiedEvent e)
        {
            _pool.Despawn(e.Creep.gameObject);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
