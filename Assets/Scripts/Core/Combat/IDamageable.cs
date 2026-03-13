using System;
using Core.Teams;
using UnityEngine;

namespace Core.Combat
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        Team Team { get; }
        Vector3 Position { get; }
        Collider Collider { get; }
        bool IsAlive { get; }
        event Action<float> DamageTaken;
        void TakeDamage(float amount, IDamageSource source);
    }
}