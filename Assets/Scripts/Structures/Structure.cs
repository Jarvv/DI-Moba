using System;
using Core.Combat;
using Core.Events;
using Core.Teams;
using UnityEngine;
using VContainer;

namespace Structures
{
    public abstract class Structure : MonoBehaviour, IDamageable
    {
        [SerializeField] private Team _team;

        private float _currentHealth;
        private bool _isActive;
        private IEventBus _eventBus;
        private Collider _collider;

        protected abstract StructureDefinitionSO Definition { get; }

        // IDamageable
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => Definition.Health;
        public Team Team => _team;
        public Vector3 Position => transform.position;
        public Collider Collider => _collider;
        public bool IsAlive => _isActive && _currentHealth > 0;

        public event Action<float> DamageTaken;

        // Visual
        private TeamVisual _teamVisual;

        protected void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected virtual void Start()
        {
            _teamVisual = GetComponentInChildren<TeamVisual>();
            _teamVisual.SetTeam(_team);

            _collider = GetComponentInChildren<Collider>();
            _isActive = true;
            _currentHealth = Definition.Health;
        }

        public void TakeDamage(float amount, IDamageSource source)
        {
            if (!_isActive) return;

            _currentHealth -= amount;
            DamageTaken?.Invoke(amount);

            if (_currentHealth <= 0)
            {
                Die(source);
            }
        }

        private void Die(IDamageSource source)
        {
            _isActive = false;
            OnDeath();
            _eventBus.Publish(new StructureDestroyedEvent
            {
                Structure = this,
                KillerTeam = source?.Team,
                GoldReward = Definition.GoldReward
            });
        }

        protected virtual void OnDeath() { }
    }
}