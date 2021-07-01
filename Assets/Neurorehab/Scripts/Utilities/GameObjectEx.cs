using UnityEngine;

namespace Neurorehab.Scripts.Utilities
{
    /// <summary>
    /// Extends GameObject funcionalities
    /// </summary>
    public static class GameObjectEx
    {
        /// <summary>
        /// Returns true if the GameObject is null
        /// </summary>
        /// <param name="go">The GameObject where this function is called from</param>
        /// <returns></returns>
        public static bool IsNull(this GameObject go)
        {
            #if UNITY_EDITOR
            return go == null;
            #endif
            return ((object) go) == null;
        }
    }
}