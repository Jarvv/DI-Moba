using Creeps.Behaviour;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Creeps
{
    public class CreepFactory : ICreepFactory
    {
        private readonly IObjectResolver _objectResolver;
        
        public CreepFactory(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        
        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints)
        {
            GameObject creepGO = _objectResolver.Instantiate(creepDefinition.Prefab, position, Quaternion.identity);
            Creep creep = creepGO.GetComponent<Creep>();
            
            IMovementBehaviour movement = new WaypointMovementBehaviour(creepDefinition.Speed);
            movement.Initialise(waypoints);
             
            creep.Initialize(movement);
            
            return creep;
        }

        public void Reinitialise(Creep creep, CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints)
        {
            creep.Reset();
            creep.transform.position = position;
            creep.gameObject.SetActive(true);

            IMovementBehaviour movement = new WaypointMovementBehaviour(creepDefinition.Speed);
            movement.Initialise(waypoints);

            creep.Initialize(movement);
        }
    }
    
    public interface ICreepFactory
    {
        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints);
        public void Reinitialise(Creep creep, CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints);
    }
}
