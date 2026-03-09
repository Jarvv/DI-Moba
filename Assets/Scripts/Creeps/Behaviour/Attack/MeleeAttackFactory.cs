using Core.Pooling;
using UnityEngine;

namespace Creeps.Behaviour.Attack
{
    [CreateAssetMenu(fileName = "MeleeAttackFactory", menuName = "MOBA/Behaviours/Melee Attack")]
    public class MeleeAttackFactory : AttackBehaviourFactory
    {
        public float Range = 2f;
        public float Damage = 5f;
        public float AttackSpeed = 2f;

        public override IAttackBehaviour Create(GameObjectPool _)
        {
            return new MeleeAttack(Range, Damage, AttackSpeed);
        }
    }
}
