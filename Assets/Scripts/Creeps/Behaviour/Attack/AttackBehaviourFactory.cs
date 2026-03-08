using UnityEngine;

namespace Creeps.Behaviour
{
    public abstract class AttackBehaviourFactory : ScriptableObject
    {
        public abstract IAttackBehaviour Create();
    }
}
