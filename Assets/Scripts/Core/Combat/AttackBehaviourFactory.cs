using Core.Pooling;
using UnityEngine;

namespace Core.Combat
{
    public abstract class AttackBehaviourFactory : ScriptableObject
    {
        public abstract IAttackBehaviour Create(GameObjectPool pool);
    }
}
