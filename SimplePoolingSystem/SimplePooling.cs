using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimplePoolingSystem
{
    /// <summary>
    /// A lightweight static object pooling utility that replaces Instantiate and Destroy
    /// with Spawn and Despawn methods.
    /// 
    /// Pools are created automatically per prefab and persist across scenes.
    /// Designed to be simple, dependency-free, and require no setup.
    /// </summary>
    public static class SimplePooling
    {
        private const string POOL_ROOT_NAME = "Object Pool";
        private const string POOL_SUFFIX = " Pool";
        private const string SPAWN_FUNCTION_NAME = "OnSpawned";
        private const string DESPAWN_FUNCTION_NAME = "OnDespawned";
        private static readonly Dictionary<int, SimplePool> pools = new();
        private static GameObject poolRootObject;
        private static bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;
            poolRootObject = new GameObject(POOL_ROOT_NAME);
            Object.DontDestroyOnLoad(poolRootObject);
        }

        #region Pool Management

        private static SimplePool GetPool(GameObject prefab)
        {
            //Checking if pool already exists
            if (pools.TryGetValue(prefab.GetInstanceID(), out SimplePool pool))
            {
                return pool;
            }

            return CreatePool(prefab);
        }

        private static SimplePool CreatePool(GameObject prefab)
        {
            //Creating parent
            GameObject parent = new(prefab.name + POOL_SUFFIX);
            parent.transform.SetParent(poolRootObject.transform);

            //Creating new pool
            SimplePool newPool = new(prefab, parent.transform);
            pools.Add(prefab.GetInstanceID(), newPool);
            return newPool;
        }

        #endregion

        #region Spawn

        /// <summary>
        /// Spawns an instance of the prefab with its default transform values.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <returns>The spawned instance.</returns>
        public static GameObject Spawn(GameObject prefab)
        {
            GameObject instance = GetPool(prefab).GetInstance();
            instance.gameObject.SetActive(true);
            instance.SendMessage(SPAWN_FUNCTION_NAME, SendMessageOptions.DontRequireReceiver);
            return instance;
        }

        /// <summary>
        /// Spawns an instance of the prefab and parents it, resetting local position.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="parent">The parent to attach the instance to.</param>
        /// <returns>The spawned instance.</returns>
        public static GameObject Spawn(GameObject prefab, Transform parent)
        {
            GameObject instance = GetPool(prefab).GetInstance();
            Transform transform = instance.transform;
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            instance.gameObject.SetActive(true);
            instance.SendMessage(SPAWN_FUNCTION_NAME, SendMessageOptions.DontRequireReceiver);
            return instance;
        }
        
        /// <summary>
        /// Spawns an instance of the prefab at a world position.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position for the instance.</param>
        /// <returns>The spawned instance.</returns>
        public static GameObject Spawn(GameObject prefab, Vector3 position)
        {
            GameObject instance = GetPool(prefab).GetInstance();
            instance.transform.position = position;
            instance.gameObject.SetActive(true);
            instance.SendMessage(SPAWN_FUNCTION_NAME, SendMessageOptions.DontRequireReceiver);
            return instance;
        }
        
        /// <summary>
        /// Spawns an instance of the prefab at a world position and rotation.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position for the instance.</param>
        /// <param name="rotation">World rotation for the instance.</param>
        /// <returns>The spawned instance.</returns>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject instance = GetPool(prefab).GetInstance();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            instance.SendMessage(SPAWN_FUNCTION_NAME, SendMessageOptions.DontRequireReceiver);
            return instance;
        }
        
        /// <summary>
        /// Spawns an instance of the prefab at a world position and rotation, then parents it.
        /// </summary>
        /// <param name="prefab">The prefab to spawn.</param>
        /// <param name="position">World position for the instance.</param>
        /// <param name="rotation">World rotation for the instance.</param>
        /// <param name="parent">The parent to attach the instance to.</param>
        /// <returns>The spawned instance.</returns>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject instance = GetPool(prefab).GetInstance();
            Transform transform = instance.transform;
            transform.SetParent(parent);
            transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            instance.SendMessage(SPAWN_FUNCTION_NAME, SendMessageOptions.DontRequireReceiver);
            return instance;
        }

        /// <summary>
        /// Spawns an instance of the prefab and returns the requested component.
        /// </summary>
        /// <param name="prefab">The prefab component to spawn.</param>
        /// <typeparam name="T">The component type to return.</typeparam>
        /// <returns>The component on the spawned instance.</returns>
        public static T Spawn<T>(T prefab) where T : Component
        {
            return Spawn(prefab.gameObject).GetComponent<T>();
        }

        /// <summary>
        /// Spawns an instance of the prefab, parents it, and returns the requested component.
        /// </summary>
        /// <param name="prefab">The prefab component to spawn.</param>
        /// <param name="parent">The parent to attach the instance to.</param>
        /// <typeparam name="T">The component type to return.</typeparam>
        /// <returns>The component on the spawned instance.</returns>
        public static T Spawn<T>(T prefab, Transform parent) where T : Component
        {
            return Spawn(prefab.gameObject, parent).GetComponent<T>();
        }
        
        /// <summary>
        /// Spawns an instance of the prefab at a world position and returns the requested component.
        /// </summary>
        /// <param name="prefab">The prefab component to spawn.</param>
        /// <param name="position">World position for the instance.</param>
        /// <typeparam name="T">The component type to return.</typeparam>
        /// <returns>The component on the spawned instance.</returns>
        public static T Spawn<T>(T prefab, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, position).GetComponent<T>();
        }

        /// <summary>
        /// Spawns an instance of the prefab at a world position and rotation, returning the requested component.
        /// </summary>
        /// <param name="prefab">The prefab component to spawn.</param>
        /// <param name="position">World position for the instance.</param>
        /// <param name="rotation">World rotation for the instance.</param>
        /// <typeparam name="T">The component type to return.</typeparam>
        /// <returns>The component on the spawned instance.</returns>
        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, position, rotation).GetComponent<T>();
        }
        
        /// <summary>
        /// Spawns an instance of the prefab at a world position and rotation, parents it, and returns the component.
        /// </summary>
        /// <param name="prefab">The prefab component to spawn.</param>
        /// <param name="position">World position for the instance.</param>
        /// <param name="rotation">World rotation for the instance.</param>
        /// <param name="parent">The parent to attach the instance to.</param>
        /// <typeparam name="T">The component type to return.</typeparam>
        /// <returns>The component on the spawned instance.</returns>
        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return Spawn(prefab.gameObject, position, rotation, parent).GetComponent<T>();
        }

        #endregion

        #region Despawn

        /// <summary>
        /// Returns an instance to its pool and deactivates it.
        /// </summary>
        /// <param name="instance">The instance to despawn.</param>
        public static void Despawn(GameObject instance)
        {
            SimplePooledInstance pooledInstance = instance.GetComponent<SimplePooledInstance>();

            if (pooledInstance == null)
            {
                throw new Exception($"Trying to release object not managed by SimplePooling: {instance.name}");
            }

            instance.SendMessage(DESPAWN_FUNCTION_NAME, SendMessageOptions.DontRequireReceiver);
            instance.gameObject.SetActive(false);
            pooledInstance.Pool.ReleaseInstance(instance);
        }

        /// <summary>
        /// Returns all instances of a prefab to its pool.
        /// </summary>
        /// <param name="prefab">The prefab whose instances should be despawned.</param>
        public static void DespawnAllInstancesOf(GameObject prefab)
        {
            if (pools.TryGetValue(prefab.GetInstanceID(), out SimplePool pool))
            {
                pool.ReleaseAllInstances();
            }
            else
            {
                throw new Exception(
                    $"Trying to release instances of a prefab that is not managed by SimplePooling: {prefab.name}");
            }
        }

        /// <summary>
        /// Returns all pooled instances to their pools.
        /// </summary>
        public static void DespawnAllInstances()
        {
            foreach (SimplePool pool in pools.Values)
            {
                pool.ReleaseAllInstances();
            }
        }

        #endregion

        #region Destroy

        /// <summary>
        /// Destroys all pooled instances of a prefab and removes its pool.
        /// </summary>
        /// <param name="prefab">The prefab whose instances should be destroyed.</param>
        public static void DestroyAllInstancesOf(GameObject prefab)
        {
            if (pools.TryGetValue(prefab.GetInstanceID(), out SimplePool pool))
            {
                pool.DestroyAllInstances();
                pools.Remove(prefab.GetInstanceID());
            }
            else
            {
                throw new Exception(
                    $"Trying to destroy instances of a prefab that is not managed by SimplePooling: {prefab.name}");
            }
        }

        /// <summary>
        /// Destroys all pooled instances and clears all pools.
        /// </summary>
        public static void DestroyAllInstances()
        {
            foreach (SimplePool pool in pools.Values)
            {
                pool.DestroyAllInstances();
            }

            //Removing pools since all instances are destroyed
            pools.Clear();
        }

        #endregion
    }
}
