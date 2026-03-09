using System;
using Core.Combat;
using UnityEngine;

namespace Creeps.Behaviour.Attack
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float maxLifetime = 5f;

        private Action<Projectile> _onComplete;
        private IDamageable _target;
        private float _damage;
        private IDamageSource _source;
        private float _lifetime;
        private bool _active;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            Collider collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        public void Initialize(IDamageable target, float damage, IDamageSource source, Action<Projectile> onComplete)
        {
            _target = target;
            _damage = damage;
            _source = source;
            _onComplete = onComplete;
            _lifetime = maxLifetime;
            _active = true;
            if (!_rigidbody.isKinematic)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        private void Update()
        {
            if (!_active) return;

            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0f)
            {
                ReturnToOwner();
                return;
            }

            if (_target == null || !_target.IsAlive)
            {
                ReturnToOwner();
                return;
            }

        }

        private void FixedUpdate()
        {
            if (!_active) return;
            if (_target == null || !_target.IsAlive) return;

            TickMove(Time.fixedDeltaTime);
        }

        private void TickMove(float deltaTime)
        {
            Vector3 currentPosition = _rigidbody.position;
            Vector3 direction = (_target.Position - currentPosition).normalized;
            Vector3 nextPosition = currentPosition + direction * speed * deltaTime;

            _rigidbody.MovePosition(nextPosition);

            if (Vector3.Distance(nextPosition, _target.Position) < 0.3f)
            {
                _target.TakeDamage(_damage, _source);
                ReturnToOwner();
            }
        }

        private void ReturnToOwner()
        {
            _active = false;
            _target = null;
            _source = null;
            _onComplete?.Invoke(this);
            _onComplete = null;
        }
    }
}
