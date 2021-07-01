using UnityEngine;

namespace Neurorehab.Scripts.Utilities
{
    public static class QuaternionMath
    {
        /// <summary>
        /// Calculates a normalized quaternion with the received components. A normalized quaternion is one that <para><c>x*x + y*y + z*z + w*w = 1</c></para>
        /// </summary>
        /// <param name="x">X component of the quaternion</param>
        /// <param name="y">Y component of the quaternion</param>
        /// <param name="z">Z component of the quaternion</param>
        /// <param name="w">W component of the quaternion</param>
        /// <returns>A normalized quaterion.</returns>
        public static Quaternion NormalizeQuaternion(float x, float y, float z, float w)
        {
            var lenghtD = 1f / (x * x + y * y + z * z + w * w);

            return new Quaternion(x * lenghtD, y * lenghtD, z * lenghtD, w * lenghtD);
        }

        /// <summary>
        /// Returns true if the Quaternions are so close that they can be considered the same Quaternion.
        /// </summary>
        /// <param name="q1">First Quaternion</param>
        /// <param name="q2">Second Quaternion</param>
        /// <returns>True if the Quaternions are so close that they can be considered the same Quaternion.</returns>
        public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2)
        {
            return !(Quaternion.Dot(q1, q2) < 0.0f);
        }

        /// <summary>
        /// Inverts the signal of each component of the quaternion
        /// </summary>
        /// <param name="quat">The quaternion that will have its components inverted</param>
        /// <returns>A new quaternion with the components of quat inverted.</returns>
        public static Quaternion InvertQuatSignal(Quaternion quat)
        {
            return new Quaternion(-quat.x, -quat.y, -quat.z, -quat.w);
        }
    }
}