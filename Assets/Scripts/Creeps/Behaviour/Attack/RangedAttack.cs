using Core.Combat;
using Core.Pooling;
using UnityEngine;

namespace Creeps.Behaviour.Attack
{
    public class RangedAttack : IAttackBehaviour
    {
        public float Range { get; }

        public float Damage { get; }

        public float AttackSpeed { get; }

        public bool IsReady => _attackCooldown <= 0f;

        private float _attackCooldown;

        private readonly GameObject _projectilePrefab;
        private readonly GameObjectPool _pool;

        public RangedAttack(float range, float damage, float attackSpeed, GameObject projectilePrefab, GameObjectPool pool)
        {
            Range = range;
            Damage = damage;
            AttackSpeed = attackSpeed;
            _projectilePrefab = projectilePrefab;
            _pool = pool;
        }

        public void Execute(Transform owner, IDamageable target, IDamageSource source)
        {
            if (!IsReady) return;
            if (!target.IsAlive) return;

            var projectile = _pool.Spawn<Projectile>(
                _projectilePrefab,
                owner.position + Vector3.up,
                Quaternion.identity);

            if (projectile != null)
            {
                projectile.Initialize(target, Damage, source, OnProjectileComplete);
            }

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

        private void OnProjectileComplete(Projectile projectile)
        {
            _pool.Despawn(projectile.gameObject);
        }
    }
}
