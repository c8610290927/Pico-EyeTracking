using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurorehab.Scripts.Utilities
{
    /// <summary>
    /// Helper class to work with enums.
    /// </summary>
    internal static class Parser
    {
        /// <summary>
        /// Returns the enum wit the same value as the string received in the parameters
        /// </summary>
        /// <typeparam name="T">The type of the enum to be used by the Parse function.</typeparam>
        /// <param name="value">The value that must be converted to enum.</param>
        /// <returns>A enum of type T with the value received in the parameters</returns>
        public static T StringToEnum<T>(string value) where T : struct
        {
            try
            {
                return (T) Enum.Parse(typeof(T), value);
            }
            catch (Exception )
            {
                //Debug.LogError(e.Message + " " + e.StackTrace);
                return default(T);
            }
        }

        /// <summary>
        /// Returns all the values of the T enum
        /// </summary>
        /// <typeparam name="T">The enum to ge the values from.</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}