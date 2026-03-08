using Core.Events;
using Core.Teams;
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

        // State
        private Team _team;
        
        // Visual
        private TeamVisual _teamVisual;
        
        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        private void Awake()
        {
            _teamVisual = GetComponentInChildren<TeamVisual>();
        }
        
        public void Initialise(IMovementBehaviour movementBehaviour, Team team)
        {
            _movementBehaviour = movementBehaviour;
            _team = team;
            
            _teamVisual.SetTeam(team);
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