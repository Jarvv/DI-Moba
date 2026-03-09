using Creeps.Behaviour.Movement;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class WaypointMovementBehaviourTests
    {
        private WaypointMovementBehaviour _movement;
        private Transform _transform;
        private Rigidbody _rigidbody;

        [SetUp]
        public void SetUp()
        {
            _movement = new WaypointMovementBehaviour(speed: 10f);
            var go = new GameObject("TestCreep");
            _transform = go.transform;
            _rigidbody = go.AddComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_transform.gameObject);
        }

        [Test]
        public void HasReachedEnd_Is_False_Before_Initialise()
        {
            Assert.IsFalse(_movement.HasReachedEnd);
        }

        [Test]
        public void HasReachedEnd_Is_False_After_Initialise_With_Waypoints()
        {
            _movement.Initialise(new[] { new Vector3(10, 0, 0) });
            Assert.IsFalse(_movement.HasReachedEnd);
        }

        [Test]
        public void Moves_Toward_First_Waypoint()
        {
            _transform.position = Vector3.zero;
            _movement.Initialise(new[] { new Vector3(10, 0, 0) });

            _movement.Tick(_rigidbody, 0.1f);

            Assert.Greater(_transform.position.x, 0f);
        }

        [Test]
        public void Reaches_Waypoint_And_Advances()
        {
            _transform.position = Vector3.zero;
            var waypoints = new[]
            {
                new Vector3(1, 0, 0),
                new Vector3(Mathf.Infinity, 0, 0)
            };
            _movement.Initialise(waypoints);

            // speed=10, dt=0.1 => 1 unit per tick. One tick lands on first waypoint, advancing index.
            _movement.Tick(_rigidbody, 0.1f);

            // Second waypoint is far away so we shouldn't have reached end
            Assert.IsFalse(_movement.HasReachedEnd);
            // Position should be at or past the first waypoint
            Assert.GreaterOrEqual(_transform.position.x, 1f);
        }

        [Test]
        public void HasReachedEnd_After_Passing_All_Waypoints()
        {
            _transform.position = Vector3.zero;
            _movement.Initialise(new[] { new Vector3(1, 0, 0) });

            // speed=10, dt=0.1 => moves exactly 1 unit, landing on the waypoint (distance=0 < 0.1 threshold)
            _movement.Tick(_rigidbody, 0.1f);

            Assert.IsTrue(_movement.HasReachedEnd);
        }

        [Test]
        public void Tick_Does_Nothing_After_Reaching_End()
        {
            _transform.position = Vector3.zero;
            _movement.Initialise(new[] { new Vector3(1, 0, 0) });

            _movement.Tick(_rigidbody, 0.1f);
            Assert.IsTrue(_movement.HasReachedEnd);

            Vector3 posAfterEnd = _transform.position;
            _movement.Tick(_rigidbody, 0.1f);

            Assert.AreEqual(posAfterEnd, _transform.position);
        }

        [Test]
        public void Reset_Restores_Waypoint_Index()
        {
            _transform.position = Vector3.zero;
            _movement.Initialise(new[] { new Vector3(1, 0, 0) });
            _movement.Tick(_rigidbody, 0.1f);
            Assert.IsTrue(_movement.HasReachedEnd);

            _movement.ResetState();
            Assert.IsFalse(_movement.HasReachedEnd);
        }
    }
}
