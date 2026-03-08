using System;
using Core.Combat;
using Core.Events;
using Core.Teams;
using Creeps.Behaviour;
using UnityEngine;
using VContainer;

namespace Creeps
{
    public class Creep : MonoBehaviour, IDamageable, IDamageSource
    {
        // Injected behaviours
        private IEventBus _eventBus;
        private ITargetFinder _targetFinder;

        // Factory behaviours
        private IMovementBehaviour _movementBehaviour;
        private IAttackBehaviour _attackBehaviour;

        // State
        private CreepDefinitionSO _definition;
        private Team _team;
        private float _currentHealth;
        private bool _isActive;

        // Visual
        private TeamVisual _teamVisual;

        // IDamageable
        public Vector3 Position => transform.position;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _definition.Health;
        public Team Team => _team;
        public bool IsAlive => _isActive && _currentHealth > 0;

        // IDamageSource
        public float Damage => _attackBehaviour.Damage;

        [Inject]
        public void Construct(IEventBus eventBus, ITargetFinder targetFinder)
        {
            _eventBus = eventBus;
            _targetFinder = targetFinder;
        }

        private void Awake()
        {
            _teamVisual = GetComponentInChildren<TeamVisual>();
        }

        public void Initialise(IMovementBehaviour movementBehaviour, IAttackBehaviour attackBehaviour, Team team, CreepDefinitionSO definition)
        {
            _movementBehaviour = movementBehaviour;
            _attackBehaviour = attackBehaviour;
            _team = team;
            _definition = definition;
            _currentHealth = _definition.Health;
            _isActive = true;

            _teamVisual.SetTeam(team);
        }

        private void Update()
        {
            _attackBehaviour.Tick(Time.deltaTime);

            Team enemyTeam = _team == Team.Red ? Team.Blue : Team.Red;
            IDamageable target = _targetFinder.FindTarget(transform.position, _attackBehaviour.Range, enemyTeam, TargetPriority.Nearest);

            if (target != null)
            {
                if (_attackBehaviour.IsReady)
                    _attackBehaviour.Execute(transform, target, this);
                return;
            }

            _movementBehaviour.Tick(transform, Time.deltaTime);
        }

        public void TakeDamage(float amount, IDamageSource source)
        {
            if (!_isActive) return;

            _currentHealth -= amount;

            if (_currentHealth <= 0)
            {
                Die(source);
            }
        }

        private void Die(IDamageSource source)
        {
            _isActive = false;
            _eventBus.Publish(new CreepDiedEvent { Creep = this });
        }

        public void ResetState()
        {
            _movementBehaviour?.ResetState();
        }
    }
}