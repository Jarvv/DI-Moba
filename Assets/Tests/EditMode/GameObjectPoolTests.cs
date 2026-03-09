using System.Collections.Generic;
using Core.Events;
using Core.Pooling;
using NUnit.Framework;
using UnityEngine;
using VContainer;
using VContainer.Diagnostics;

namespace Tests.EditMode
{
    public class GameObjectPoolTests
    {
        private GameObjectPool _pool;
        private GameObject _prefab;
        private readonly List<GameObject> _cleanup = new();

        [SetUp]
        public void SetUp()
        {
            _pool = new GameObjectPool(new StubResolver(), new EventBus());
            _prefab = new GameObject("Prefab");
            _cleanup.Add(_prefab);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var go in _cleanup)
                Object.DestroyImmediate(go);
            _cleanup.Clear();
        }

        private GameObject Spawn(Vector3 position = default)
        {
            var go = _pool.Spawn(_prefab, position, Quaternion.identity);
            _cleanup.Add(go);
            return go;
        }

        [Test]
        public void Spawn_Creates_New_Instance_When_Pool_Empty()
        {
            var instance = Spawn();

            Assert.IsNotNull(instance);
            Assert.AreNotSame(_prefab, instance);
        }

        [Test]
        public void Spawn_Sets_Position()
        {
            var pos = new Vector3(5, 10, 15);
            var instance = Spawn(pos);

            Assert.AreEqual(pos, instance.transform.position);
        }

        [Test]
        public void Despawn_Deactivates_GameObject()
        {
            var instance = Spawn();

            _pool.Despawn(instance);

            Assert.IsFalse(instance.activeSelf);
        }

        [Test]
        public void Spawn_Reuses_Despawned_Instance()
        {
            var instance = Spawn();
            _pool.Despawn(instance);

            var reused = _pool.Spawn(_prefab, Vector3.zero, Quaternion.identity);

            Assert.AreSame(instance, reused);
        }

        [Test]
        public void Spawn_Reactivates_Pooled_Instance()
        {
            var instance = Spawn();
            _pool.Despawn(instance);

            _pool.Spawn(_prefab, Vector3.zero, Quaternion.identity);

            Assert.IsTrue(instance.activeSelf);
        }

        [Test]
        public void Spawn_Repositions_Pooled_Instance()
        {
            var instance = Spawn(Vector3.zero);
            _pool.Despawn(instance);

            var pos = new Vector3(99, 0, 0);
            _pool.Spawn(_prefab, pos, Quaternion.identity);

            Assert.AreEqual(pos, instance.transform.position);
        }

        [Test]
        public void Despawned_Instances_Reused_In_FIFO_Order()
        {
            var first = Spawn();
            var second = Spawn();

            _pool.Despawn(first);
            _pool.Despawn(second);

            var reused1 = _pool.Spawn(_prefab, Vector3.zero, Quaternion.identity);
            var reused2 = _pool.Spawn(_prefab, Vector3.zero, Quaternion.identity);

            Assert.AreSame(first, reused1);
            Assert.AreSame(second, reused2);
        }

        [Test]
        public void Different_Prefabs_Have_Separate_Pools()
        {
            var prefabB = new GameObject("PrefabB");
            _cleanup.Add(prefabB);

            var instanceA = _pool.Spawn(_prefab, Vector3.zero, Quaternion.identity);
            _cleanup.Add(instanceA);
            _pool.Despawn(instanceA);

            var instanceB = _pool.Spawn(prefabB, Vector3.zero, Quaternion.identity);
            _cleanup.Add(instanceB);

            // Should NOT reuse instanceA since it's a different prefab
            Assert.AreNotSame(instanceA, instanceB);
        }

        [Test]
        public void Spawn_Generic_Returns_Component()
        {
            var instance = _pool.Spawn<Transform>(_prefab, Vector3.zero, Quaternion.identity);
            _cleanup.Add(instance.gameObject);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<Transform>(instance);
        }

        /// <summary>
        /// Stub that clones the prefab without VContainer.
        /// </summary>
        private class StubResolver : IObjectResolver
        {
            public object ApplicationOrigin => null;
            public DiagnosticsCollector Diagnostics { get; set; }

            public object Resolve(System.Type type, object key = null) => null;
            public object Resolve(Registration registration) => null;
            public IScopedObjectResolver CreateScope(System.Action<IContainerBuilder> installation = null) => null;
            public void Inject(object instance) { }
            public bool TryResolve(System.Type type, out object resolved, object key = null) { resolved = null; return false; }
            public bool TryGetRegistration(System.Type type, out Registration registration, object key = null)
            {
                registration = default;
                return false;
            }
            public void Dispose() { }
        }
    }
}
