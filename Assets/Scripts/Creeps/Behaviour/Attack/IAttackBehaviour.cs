using Core.Combat;
using UnityEngine;

namespace Creeps.Behaviour.Attack
{
    public interface IAttackBehaviour
    {
        float Range { get; }
        float Damage { get; }
        float AttackSpeed { get; }
        bool IsReady { get; }
        void Tick(float deltaTime);
        void Execute(Transform owner, IDamageable target, IDamageSource source);
        void ResetState();
    }
}