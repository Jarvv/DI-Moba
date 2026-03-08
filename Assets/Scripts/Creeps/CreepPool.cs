using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Creeps
{
    public class CreepPool : ICreepPool
    {
        private readonly ICreepFactory _creepFactory;
        private readonly Queue<Creep> _inactive = new Queue<Creep>();
        private readonly List<Creep> _active = new List<Creep>();

        public CreepPool(ICreepFactory creepFactory)
        {
            _creepFactory = creepFactory;
        }
        
        public Creep SpawnCreep(CreepDefinitionSO definition, Vector3 position)
        {
            Creep creep;
            
            if (_inactive.Count > 0)
            {
                creep = _inactive.Dequeue();
                _creepFactory.Reinitialise(creep, definition, position);
            }
            else
            {
                creep = _creepFactory.Create(definition, position);
            }
            
            _active.Add(creep);
            
            return creep;
        }
        
        public void DespawnCreep(Creep creep)
        {
            creep.gameObject.SetActive(false);
            _active.Remove(creep);
            _inactive.Enqueue(creep);
        }
    }
    
    public interface ICreepPool
    {
        public Creep SpawnCreep(CreepDefinitionSO definition, Vector3 position);
        public void DespawnCreep(Creep creep);
    }
}