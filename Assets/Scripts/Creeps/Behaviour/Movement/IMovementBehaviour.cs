using UnityEngine;

namespace Creeps.Behaviour.Movement
{
    public interface IMovementBehaviour
    {
        public void Initialise(Vector3[] waypoints);
        public void Tick(Rigidbody creepRigidbody, float deltaTime);
        public void ResetState();
    }
}
