using Core.Pooling;
using UnityEngine;

namespace Creeps.Behaviour.Attack
{
    [CreateAssetMenu(fileName = "RangedAttackFactory", menuName = "MOBA/Behaviours/Ranged Attack")]
    public class RangedAttackFactory : AttackBehaviourFactory
    {
        public float Range = 5f;
        public float Damage = 2f;
        public float AttackSpeed = 2f;
        public GameObject ProjectilePrefab;

        public override IAttackBehaviour Create(GameObjectPool pool)
        {
            return new RangedAttack(Range, Damage, AttackSpeed, ProjectilePrefab, pool);
        }
    }
}
