using System.Collections.Generic;
using UnityEngine;

namespace SimplePoolingSystem
{
    /// <summary>
    /// Internal pool implementation that tracks active and inactive instances for a prefab.
    /// </summary>
    internal sealed class SimplePool
    {
        
        private readonly GameObject prefab;
        private readonly Transform parent;
        private readonly List<GameObject> allInstances = new();
        private readonly Stack<GameObject> inactiveInstances = new();

        internal SimplePool(GameObject prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        internal GameObject GetInstance()
        {
            if (inactiveInstances.Count == 0) return CreateNewInstance();

            GameObject instance = inactiveInstances.Pop();
            return instance;
        }

        internal void ReleaseInstance(GameObject instance)
        {
            if (inactiveInstances.Contains(instance)) return;
            
            instance.transform.SetParent(parent);
            inactiveInstances.Push(instance);
        }

        internal void ReleaseAllInstances()
        {
            for (int i = 0; i < allInstances.Count; i++)
            {
                ReleaseInstance(allInstances[i]);
            }
        }

        internal void DestroyAllInstances()
        {
            //HandleDestroyedInstance will be called by SimplePooledInstance
            for (int i = 0; i < allInstances.Count; i++)
            {
                Object.Destroy(allInstances[i]);
            }

            Object.Destroy(parent.gameObject);
            allInstances.Clear();
            inactiveInstances.Clear();
        }

        internal void HandleDestroyedInstance(int instanceID)
        {
            for (int i = 0; i < allInstances.Count; i++)
            {
                if (allInstances[i].GetInstanceID() != instanceID) continue;
                allInstances.RemoveAt(i);
                break;
            }
        }

        private GameObject CreateNewInstance()
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            SimplePooledInstance pooledInstance = instance.AddComponent<SimplePooledInstance>();
            pooledInstance.Pool = this;
            pooledInstance.Prefab = prefab;
            allInstances.Add(instance);
            return instance;
        }
    }
}
