using Core.Events;
using Core.Teams;
using Creeps;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class CreepPoolTests
    {
        private StubCreepFactory _factory;
        private CreepPool _pool;

        [SetUp]
        public void SetUp()
        {
            _factory = new StubCreepFactory();
            _pool = new CreepPool(_factory, new EventBus());
        }

        [TearDown]
        public void TearDown()
        {
            _factory.DestroyAll();
        }

        [Test]
        public void SpawnCreep_Creates_Via_Factory_When_Pool_Empty()
        {
            var creep = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);

            Assert.IsNotNull(creep);
            Assert.AreEqual(1, _factory.CreateCount);
        }

        [Test]
        public void SpawnCreep_Reuses_Despawned_Creep()
        {
            var creep = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);
            _pool.DespawnCreep(creep);

            var reused = _pool.SpawnCreep(null, Vector3.zero, null, Team.Red);

            Assert.AreSame(creep, reused);
            Assert.AreEqual(1, _factory.CreateCount);
            Assert.AreEqual(1, _factory.ReinitialiseCount);
        }

        [Test]
        public void DespawnCreep_Deactivates_GameObject()
        {
            var creep = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);

            _pool.DespawnCreep(creep);

            Assert.IsFalse(creep.gameObject.activeSelf);
        }

        [Test]
        public void SpawnCreep_Reactivates_Pooled_Creep()
        {
            var creep = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);
            _pool.DespawnCreep(creep);
            Assert.IsFalse(creep.gameObject.activeSelf);

            _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);

            // Reinitialise in the stub reactivates the GO
            Assert.IsTrue(creep.gameObject.activeSelf);
        }

        [Test]
        public void Multiple_Despawns_Are_Reused_In_FIFO_Order()
        {
            var first = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);
            var second = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);

            _pool.DespawnCreep(first);
            _pool.DespawnCreep(second);

            var reused1 = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);
            var reused2 = _pool.SpawnCreep(null, Vector3.zero, null, Team.Blue);

            Assert.AreSame(first, reused1);
            Assert.AreSame(second, reused2);
            Assert.AreEqual(2, _factory.CreateCount);
        }

        /// <summary>
        /// Minimal stub that creates real Creep MonoBehaviours on bare GameObjects.
        /// Avoids the full prefab/TeamVisual dependency chain.
        /// </summary>
        private class StubCreepFactory : ICreepFactory
        {
            private readonly System.Collections.Generic.List<GameObject> _created = new();

            public int CreateCount { get; private set; }
            public int ReinitialiseCount { get; private set; }

            public Creep Create(CreepDefinitionSO def, Vector3 pos, Vector3[] waypoints, Team team)
            {
                CreateCount++;
                var go = new GameObject("PoolTestCreep");
                go.transform.position = pos;
                var creep = go.AddComponent<Creep>();
                _created.Add(go);
                return creep;
            }

            public void Reinitialise(Creep creep, CreepDefinitionSO def, Vector3 pos, Vector3[] waypoints, Team team)
            {
                ReinitialiseCount++;
                creep.transform.position = pos;
                creep.gameObject.SetActive(true);
            }

            public void DestroyAll()
            {
                foreach (var go in _created)
                    Object.DestroyImmediate(go);
                _created.Clear();
            }
        }
    }
}
