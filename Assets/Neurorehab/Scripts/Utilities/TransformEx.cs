using UnityEngine;

namespace Neurorehab.Scripts.Utilities
{
    /// <summary>
    /// Extension class for Unity3d class Transform
    /// </summary>
    public static class TransformEx
    {
        /// <summary>
        /// Extention method for Transform objects that destroys all of its children.
        /// </summary>
        /// <param name="transform">The transform to have its children destroyed</param>
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
                Object.Destroy(child.gameObject);
        }
    }
}