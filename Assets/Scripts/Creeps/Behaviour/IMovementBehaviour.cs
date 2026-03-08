using UnityEngine;

namespace Creeps.Behaviour
{
    public interface IMovementBehaviour
    {
        public void Initialise(Vector3[] waypoints);
        public void Tick(Transform creepTransform, float deltaTime);
        public void Reset();
    }
}