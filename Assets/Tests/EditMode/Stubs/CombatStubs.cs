using System;
using Core.Combat;
using Core.Teams;
using UnityEngine;

namespace Tests.EditMode.Stubs
{
    public class StubDamageable : IDamageable
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; }
        public Team Team { get; }
        public Vector3 Position { get; }
        public Collider Collider => null;
        public bool IsAlive => CurrentHealth > 0;
        public event Action<float> DamageTaken;

        public StubDamageable(float health, Team team, Vector3 position = default)
        {
            CurrentHealth = health;
            MaxHealth = health;
            Team = team;
            Position = position;
        }

        public void TakeDamage(float amount, IDamageSource source)
        {
            CurrentHealth -= amount;
            DamageTaken?.Invoke(amount);
        }
    }

    public class StubDamageSource : IDamageSource
    {
        public float Damage { get; }
        public Team Team { get; }

        public StubDamageSource(float damage, Team team)
        {
            Damage = damage;
            Team = team;
        }
    }
}
