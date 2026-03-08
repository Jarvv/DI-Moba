using Core.Teams;
using Creeps.Behaviour;
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

        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints, Team team)
        {
            GameObject creepGO = _objectResolver.Instantiate(creepDefinition.Prefab, position, Quaternion.identity);
            Creep creep = creepGO.GetComponent<Creep>();

            IMovementBehaviour movement = new WaypointMovementBehaviour(creepDefinition.Speed);
            movement.Initialise(waypoints);

            IAttackBehaviour attack = creepDefinition.AttackFactory.Create();

            creep.Initialise(movement, attack, team, creepDefinition);

            return creep;
        }

        public void Reinitialise(Creep creep, CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints, Team team)
        {
            creep.ResetState();
            creep.transform.position = position;
            creep.gameObject.SetActive(true);

            IMovementBehaviour movement = new WaypointMovementBehaviour(creepDefinition.Speed);
            movement.Initialise(waypoints);

            IAttackBehaviour attack = creepDefinition.AttackFactory.Create();

            creep.Initialise(movement, attack, team, creepDefinition);
        }
    }

    public interface ICreepFactory
    {
        public Creep Create(CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints, Team team);
        public void Reinitialise(Creep creep, CreepDefinitionSO creepDefinition, Vector3 position, Vector3[] waypoints, Team team);
    }
}
