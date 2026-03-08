using Core.Events;
using UnityEngine;
using VContainer;

namespace Creeps
{
    public class Creep : MonoBehaviour
    {
        // Injected behaviours
        private IEventBus _eventBus;
        
        // Factory behaviours
        
        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        public void Reset()
        {
            
        }
    }
}