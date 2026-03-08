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
        bool IsAlive { get; }
        void TakeDamage(float amount, IDamageSource source);
    }
}