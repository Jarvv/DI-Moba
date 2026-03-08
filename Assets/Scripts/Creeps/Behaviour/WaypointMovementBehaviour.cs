using UnityEngine;

namespace Creeps.Behaviour
{
    public class WaypointMovementBehaviour : IMovementBehaviour
    {
        private readonly float _speed;
        private const float _waypointThreshold = 0.1f;
    
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
      
        public void Tick(Transform creepTransform, float deltaTime)
        {
            if (HasReachedEnd || _waypoints == null) return;

            Vector3 target = _waypoints[_currentWaypointIndex];
            Vector3 direction = (target - creepTransform.position).normalized;

            creepTransform.position += direction * _speed * deltaTime;

            // Rotate to face movement direction
            if (direction != Vector3.zero)
                creepTransform.rotation = Quaternion.Slerp(
                    creepTransform.rotation,
                    Quaternion.LookRotation(direction),
                    deltaTime * 10f);

            if (Vector3.Distance(creepTransform.position, target) < _waypointThreshold)
                _currentWaypointIndex++;
        }
        
        public void ResetState()
        {
            _currentWaypointIndex = 0;
        }

    }
}