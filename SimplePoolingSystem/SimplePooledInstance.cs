using UnityEngine;

namespace SimplePoolingSystem
{
    /// <summary>
    /// Marker component automatically added to pooled instances to connect them back to their pool.
    /// </summary>
    [AddComponentMenu("")]
    internal sealed class SimplePooledInstance : MonoBehaviour
    {
        internal GameObject Prefab { get; set; }
        internal SimplePool Pool { get; set; }
        
        private void OnDestroy()
        {
            Pool.HandleDestroyedInstance(GetInstanceID());
        }
    }
}
