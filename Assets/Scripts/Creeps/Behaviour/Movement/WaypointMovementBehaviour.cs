using UnityEngine;

namespace Creeps.Behaviour.Movement
{
    public class WaypointMovementBehaviour : IMovementBehaviour
    {
        private readonly float _speed;
        private const float _waypointThreshold = 0.1f;

        public float Speed => _speed;

        private Vector3[] _waypoints;
        private int _currentWaypointIndex;

        public bool HasReachedEnd => _waypoints != null && _currentWaypointIndex >= _waypoints.Length;

        public WaypointMovementBehaviour(float speed)
        {
            _speed = speed;
        }

        public void Initialise(Vector3[] waypoints)
        {
            _waypoints = waypoints;
            _currentWaypointIndex = 0;
        }

        public void Tick(Rigidbody creepRigidbody, float deltaTime)
        {
            if (HasReachedEnd || _waypoints == null) return;

            Vector3 currentPosition = creepRigidbody.position;
            Vector3 target = _waypoints[_currentWaypointIndex];

            // Movement is lane-planar: ignore Y so child-body offset doesn't stall waypoint progression.
            Vector3 currentPlanar = new Vector3(currentPosition.x, 0f, currentPosition.z);
            Vector3 targetPlanar = new Vector3(target.x, 0f, target.z);
            Vector3 direction = (targetPlanar - currentPlanar).normalized;

            Vector3 nextPosition = currentPosition + direction * _speed * deltaTime;
            creepRigidbody.position = nextPosition;

            // Rotate to face movement direction
            if (direction != Vector3.zero)
            {
                creepRigidbody.rotation = Quaternion.Slerp(
                    creepRigidbody.rotation,
                    Quaternion.LookRotation(direction),
                    deltaTime * 10f);
            }

            Vector3 nextPlanar = new Vector3(nextPosition.x, 0f, nextPosition.z);
            if ((nextPlanar - targetPlanar).sqrMagnitude < _waypointThreshold * _waypointThreshold)
                _currentWaypointIndex++;
        }

        public void ResetState()
        {
            _currentWaypointIndex = 0;
        }

    }
}
