using Core.Combat;
using UnityEngine;

namespace Creeps.Behaviour.Attack
{
    public class MeleeAttack : IAttackBehaviour
    {
        public float Range { get; }

        public float Damage { get; }

        public float AttackSpeed { get; }

        public bool IsReady => _attackCooldown <= 0f;

        private float _attackCooldown;

        public MeleeAttack(float range, float damage, float attackSpeed)
        {
            Range = range;
            Damage = damage;
            AttackSpeed = attackSpeed;
        }

        public void Execute(Transform owner, IDamageable target, IDamageSource source)
        {
            if (!IsReady) return;
            if (!target.IsAlive) return;

            target.TakeDamage(Damage, source);
            _attackCooldown = 1f / AttackSpeed;
        }

        public void Tick(float deltaTime)
        {
            if (_attackCooldown > 0f)
            {
                _attackCooldown -= deltaTime;
            }
        }

        public void ResetState() => _attackCooldown = 0;
    }
}
