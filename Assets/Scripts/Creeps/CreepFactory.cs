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
        
        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position)
        {
            GameObject creepGO = _objectResolver.Instantiate(creepDefinition.Prefab, position, Quaternion.identity);
            Creep creep = creepGO.GetComponent<Creep>();
            
            return creep;
        }

        public void Reinitialise(Creep creep, CreepDefinitionSO creepDefinition, Vector3 position)
        {
            creep.transform.position = position;
            creep.gameObject.SetActive(true);
            creep.Reset();
        }
    }
    
    public interface ICreepFactory
    {
        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position);
        public void Reinitialise(Creep creep, CreepDefinitionSO creepDefinition, Vector3 position);
    }
}
