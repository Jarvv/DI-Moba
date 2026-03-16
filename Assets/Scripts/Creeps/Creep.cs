using System;
using Core.Combat;
using Core.Events;
using Core.State;
using Core.Teams;
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
        private IGameState _gameState;

        // Factory behaviours
        private IMovementBehaviour _movementBehaviour;
        private IAttackBehaviour _attackBehaviour;

        // State
        private CreepDefinitionSO _definition;
        private Team _team;
        private float _currentHealth;
        private bool _isActive;
        private bool _shouldMove;
        private IDamageable _chaseTarget;
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

        // IDamageable events
        public event Action<float> DamageTaken;

        // IDamageSource
        public float Damage => _attackBehaviour.Damage;

        [Inject]
        public void Construct(IEventBus eventBus, ITargetFinder targetFinder, IGameState gameState)
        {
            _eventBus = eventBus;
            _targetFinder = targetFinder;
            _gameState = gameState;
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
            if (!_isActive || _gameState.IsGameOver) return;

            _attackBehaviour.Tick(Time.deltaTime);

            Team enemyTeam = _team == Team.Red ? Team.Blue : Team.Red;
            IDamageable acquiredTarget = _targetFinder.FindTarget(Position, _definition.AcquisitionRange, enemyTeam, TargetPriority.Nearest);
            IDamageable attackTarget = _targetFinder.FindTarget(Position, _attackBehaviour.Range, enemyTeam, TargetPriority.Nearest);

            if (attackTarget != null && _attackBehaviour.IsReady)
            {
                _attackBehaviour.Execute(_rigidbody.transform, attackTarget, this);
            }

            _chaseTarget = (acquiredTarget != null && attackTarget == null) ? acquiredTarget : null;
            _shouldMove = attackTarget == null;
        }

        private void FixedUpdate()
        {
            if (!_isActive || _gameState.IsGameOver) return;

            ApplySeparation(Time.fixedDeltaTime);

            if (!_shouldMove) return;

            if (_chaseTarget != null)
            {
                ChaseTarget(Time.fixedDeltaTime);
            }
            else if (_movementBehaviour != null)
            {
                _movementBehaviour.Tick(_rigidbody, Time.fixedDeltaTime);
            }
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

        private void ChaseTarget(float deltaTime)
        {
            Vector3 currentPos = _rigidbody.position;
            Vector3 targetPos = _chaseTarget.Position;
            currentPos.y = 0f;
            targetPos.y = 0f;

            Vector3 direction = (targetPos - currentPos).normalized;
            _rigidbody.position += direction * (_movementBehaviour.Speed * deltaTime);

            if (direction != Vector3.zero)
            {
                _rigidbody.rotation = Quaternion.Slerp(
                    _rigidbody.rotation,
                    Quaternion.LookRotation(direction),
                    deltaTime * 10f);
            }
        }

        private static readonly Collider[] _nearbyBuffer = new Collider[8];
        private const float SeparationRadius = 0.6f;
        private const float SeparationStrength = 2f;

        private void ApplySeparation(float deltaTime)
        {
            int count = Physics.OverlapSphereNonAlloc(
                _rigidbody.position, SeparationRadius, _nearbyBuffer);

            Vector3 push = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                if (_nearbyBuffer[i].attachedRigidbody == _rigidbody) continue;

                Vector3 away = _rigidbody.position - _nearbyBuffer[i].transform.position;
                away.y = 0f;
                float dist = away.magnitude;

                if (dist > 0f && dist < SeparationRadius)
                {
                    push += away.normalized * (1f - dist / SeparationRadius);
                }
            }

            if (push.sqrMagnitude > 0f)
            {
                _rigidbody.position += push * (SeparationStrength * deltaTime);
            }
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
