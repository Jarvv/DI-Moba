using Core.Combat;
using Core.Pooling;
using Core.Teams;
using Creeps.Behaviour.Movement;
using System;
using UnityEngine;

namespace Creeps
{
    public class CreepFactory : ICreepFactory
    {
        private readonly GameObjectPool _pool;

        public CreepFactory(GameObjectPool pool)
        {
            _pool = pool;
        }

        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints, Team team)
        {
            Creep creep = _pool.Spawn<Creep>(creepDefinition.Prefab, position, Quaternion.identity);

            creep.ResetState();

            IMovementBehaviour movement = creepDefinition.MovementFactory.Create();
            movement.Initialise(waypoints);

            IAttackBehaviour attack = creepDefinition.AttackFactory.Create(_pool);

            creep.Initialise(movement, attack, team, creepDefinition);

            return creep;
        }
    }

    public interface ICreepFactory
    {
        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints, Team team);
    }
}
