using Core.Pooling;
using UnityEngine;

namespace Creeps.Behaviour.Attack
{
    public abstract class AttackBehaviourFactory : ScriptableObject
    {
        public abstract IAttackBehaviour Create(GameObjectPool pool);
    }
}
