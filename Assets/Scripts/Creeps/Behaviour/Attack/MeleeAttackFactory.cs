using UnityEngine;

namespace Creeps.Behaviour
{
    [CreateAssetMenu(fileName = "MeleeAttackFactory", menuName = "MOBA/Behaviours/Melee Attack")]
    public class MeleeAttackFactory : AttackBehaviourFactory
    {
        public float Range = 2f;
        public float Damage = 5f;
        public float AttackSpeed = 2f;

        public override IAttackBehaviour Create()
        {
            return new MeleeAttack(Range, Damage, AttackSpeed);
        }
    }
}
