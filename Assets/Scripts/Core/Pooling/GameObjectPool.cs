using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Pooling
{
    public class GameObjectPool
    {
        private readonly IObjectResolver _resolver;
        private readonly Transform _root;
        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();

        public GameObjectPool(IObjectResolver resolver, IEventBus eventBus)
        {
            _resolver = resolver;
            eventBus.Subscribe<DespawnRequestEvent>(e => Despawn(e.GameObject));

            _root = new GameObject("[Pool]").transform;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject instance;

            if (TryGetFromPool(prefab, out instance))
            {
                instance.transform.SetParent(null);
                instance.transform.SetPositionAndRotation(position, rotation);
                instance.SetActive(true);
            }
            else
            {
                instance = _resolver.Instantiate(prefab, position, rotation);
                _instanceToPrefab[instance] = prefab;
            }

            return instance;
        }

        public T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            var go = Spawn(prefab, position, rotation);
            return go.GetComponent<T>();
        }

        public void Despawn(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(_root);

            if (_instanceToPrefab.TryGetValue(instance, out var prefab))
            {
                GetOrCreateQueue(prefab).Enqueue(instance);
            }
        }

        private bool TryGetFromPool(GameObject prefab, out GameObject instance)
        {
            if (_pools.TryGetValue(prefab, out var queue) && queue.Count > 0)
            {
                instance = queue.Dequeue();
                return true;
            }

            instance = null;
            return false;
        }

        private Queue<GameObject> GetOrCreateQueue(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                _pools[prefab] = queue;
            }
            return queue;
        }
    }
}