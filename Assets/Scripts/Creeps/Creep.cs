using Core.Events;
using Creeps.Behaviour;
using UnityEngine;
using VContainer;

namespace Creeps
{
    public class Creep : MonoBehaviour
    {
        // Injected behaviours
        private IEventBus _eventBus;
        
        // Factory behaviours
        private IMovementBehaviour _movementBehaviour;
        
        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        public void Initialize(IMovementBehaviour movementBehaviour)
        {
            _movementBehaviour = movementBehaviour;
        }
        
        private void Update()
        {
            _movementBehaviour.Tick(transform, Time.deltaTime);
        }
        
        public void Reset()
        {
            _movementBehaviour.Reset();
        }
    }
}