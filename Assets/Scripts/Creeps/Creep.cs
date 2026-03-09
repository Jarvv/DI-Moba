using Core.Combat;
using Core.Events;
using Core.Teams;
using Creeps.Behaviour.Attack;
using Creeps.Behaviour.Movement;
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
        private bool _shouldMove;
        private Rigidbody _rigidbody;
        private Vector3 _bodyInitialLocalPosition;
        private Quaternion _bodyInitialLocalRotation;

        // Visual
        private TeamVisual _teamVisual;

        // IDamageable
        public Vector3 Position => _rigidbody.position;
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
            _rigidbody = GetComponentInChildren<Rigidbody>();

            _bodyInitialLocalPosition = _rigidbody.transform.localPosition;
            _bodyInitialLocalRotation = _rigidbody.transform.localRotation;

            ConfigureRigidbody(_rigidbody);
        }

        public void Initialise(IMovementBehaviour movementBehaviour, IAttackBehaviour attackBehaviour, Team team, CreepDefinitionSO definition)
        {
            _movementBehaviour = movementBehaviour;
            _attackBehaviour = attackBehaviour;
            _team = team;
            _definition = definition;
            _currentHealth = _definition.Health;
            _isActive = true;
            _shouldMove = true;

            _rigidbody.position = transform.TransformPoint(_bodyInitialLocalPosition);
            _rigidbody.rotation = transform.rotation * _bodyInitialLocalRotation;

            _teamVisual.SetTeam(team);
        }

        private void Update()
        {
            _attackBehaviour.Tick(Time.deltaTime);

            Team enemyTeam = _team == Team.Red ? Team.Blue : Team.Red;
            IDamageable target = _targetFinder.FindTarget(Position, _attackBehaviour.Range, enemyTeam, TargetPriority.Nearest);
            bool targetInRange = target != null && IsTargetInRange(target);

            if (targetInRange && _attackBehaviour.IsReady)
            {
                _attackBehaviour.Execute(_rigidbody.transform, target, this);
            }

            // Keep moving until we are actually in attack range.
            _shouldMove = !targetInRange;
        }

        private void FixedUpdate()
        {
            if (!_isActive) return;
            if (_movementBehaviour == null) return;
            if (!_shouldMove) return;

            _movementBehaviour.Tick(_rigidbody, Time.fixedDeltaTime);
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
            _eventBus.Publish(new CreepDiedEvent
            {
                Creep = this,
                KillerTeam = source != null ? source.Team : null,
                GoldReward = _definition.GoldReward
            });
        }

        public void ResetState()
        {
            _movementBehaviour?.ResetState();
            _shouldMove = true;
        }

        private bool IsTargetInRange(IDamageable target)
        {
            Vector3 from = Position;
            Vector3 to = target.Position;
            from.y = 0f;
            to.y = 0f;

            float range = _attackBehaviour.Range;
            return (to - from).sqrMagnitude <= range * range;
        }

        private static void ConfigureRigidbody(Rigidbody rb)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.constraints =
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
        }
    }
}
