using UnityEngine;

namespace Creeps.Behaviour.Movement
{
    [CreateAssetMenu(fileName = "WaypointMovementFactory", menuName = "MOBA/Behaviours/Waypoint Movement")]
    public class WaypointMovementFactory : MovementBehaviourFactory
    {
        public float Speed = 1f;

        public override IMovementBehaviour Create()
        {
            return new WaypointMovementBehaviour(Speed);
        }
    }
}
