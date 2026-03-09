using UnityEngine;

namespace Creeps.Behaviour.Movement
{
    public abstract class MovementBehaviourFactory : ScriptableObject
    {
        public abstract IMovementBehaviour Create();
    }
}
