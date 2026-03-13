using Core.Combat;
using Core.Events;
using Core.Pooling;
using Core.Teams;
using UnityEngine;
using VContainer;

namespace Structures.Towers
{
    public class Tower : Structure, IDamageSource
    {
        [SerializeField] private TowerDefinitionSO _towerDefinition;

        private ITargetFinder _targetFinder;
        private GameObjectPool _pool;
        private IAttackBehaviour _attackBehaviour;

        protected override StructureDefinitionSO Definition => _towerDefinition;

        // IDamageSource
        public float Damage => _attackBehaviour.Damage;

        [Inject]
        public void Construct(IEventBus eventBus, ITargetFinder targetFinder, GameObjectPool pool)
        {
            base.Construct(eventBus);
            _targetFinder = targetFinder;
            _pool = pool;
        }

        protected override void Start()
        {
            base.Start();
            _attackBehaviour = _towerDefinition.AttackFactory.Create(_pool);
        }

        private void Update()
        {
            if (!IsAlive) return;

            _attackBehaviour.Tick(Time.deltaTime);

            Team enemyTeam = Team == Team.Red ? Team.Blue : Team.Red;
            IDamageable target = _targetFinder.FindTarget(
                Position, _attackBehaviour.Range, enemyTeam, TargetPriority.Nearest);

            if (target != null && _attackBehaviour.IsReady)
            {
                _attackBehaviour.Execute(transform, target, this);
            }
        }
    }
}
