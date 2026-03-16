using Core.Events;
using UnityEngine;
using VContainer;

namespace Structures.Nexus
{
    public class Nexus : Structure
    {
        [SerializeField] private NexusDefinitionSO _nexusDefinition;

        protected override StructureDefinitionSO Definition => _nexusDefinition;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            base.Construct(eventBus);
        }
    }
}
