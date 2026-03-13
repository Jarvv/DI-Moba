using UnityEngine;

namespace Creeps.Behaviour.Movement
{
    public interface IMovementBehaviour
    {
        float Speed { get; }
        void Initialise(Vector3[] waypoints);
        void Tick(Rigidbody creepRigidbody, float deltaTime);
        void ResetState();
    }
}
