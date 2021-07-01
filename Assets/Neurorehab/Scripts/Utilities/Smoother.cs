using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.Devices;
using UnityEngine;

namespace Neurorehab.Scripts.Utilities
{
    public static class Smoother
    {
        /// <summary>
        /// Calculates a new Vector3 with the average values of all Vector3 existing in the queue.
        /// </summary>
        /// <param name="queue">The vector3 queue to be averaged.</param>
        /// <returns>A new vector3 with all the average values of all Vector3 existing in the queue.</returns>
        public static Vector3 Average(Queue<UdpPosition> queue)
        {
            var avgPosotion = Vector3.zero;

            if (queue.Count == 0) return avgPosotion;

            foreach (var pos in queue)
                avgPosotion += pos.Position;

            return avgPosotion / queue.Count;
        }

        /// <summary>
        /// Calculates a new Quaternion with the average values of all Quaternion existing in the queue.
        /// </summary>
        /// <param name="queue">The Quaternion queue to be averaged.</param>
        /// <returns>A new Quaternion with all the average values of all Quaternion existing in the queue.</returns>
        internal static Quaternion Average(Queue<UdpRotation> queue)
        {
            var x = 0f;
            var y = 0f;
            var z = 0f;
            var w = 0f;

            if (queue.Count == 0) return Quaternion.identity;

            var addDet = 1f / queue.Count;

            foreach (var quat in queue)
            {
                var tempQuat = QuaternionMath.AreQuaternionsClose(queue.Peek().Rotation, quat.Rotation) == false ? QuaternionMath.InvertQuatSignal(quat.Rotation) : quat.Rotation;

                x += tempQuat.x * addDet;
                y += tempQuat.y * addDet;
                z += tempQuat.z * addDet;
                w += tempQuat.w * addDet;
            }

            return QuaternionMath.NormalizeQuaternion(x, y, z, w);
        }

        /// <summary>
        /// Calculates a new Vector2 with the average values of all Vector2 existing in the queue.
        /// </summary>
        /// <param name="queue">The Vector2 queue to be averaged.</param>
        /// <returns>A new Vector2 with all the average values of all Vector2 existing in the queue.</returns>
        public static Vector2 Average(Queue<Vector2> queue)
        {
            var avgPosotion = Vector2.zero;

            if (queue.Count == 0) return avgPosotion;


            foreach (var pos in queue)
                avgPosotion += pos;

            return avgPosotion / queue.Count;
        }

        /// <summary>
        /// Calculates a new float with the average values of all float existing in the queue.
        /// </summary>
        /// <param name="queue">The float queue to be averaged.</param>
        /// <returns>A new float with all the average values of all float existing in the queue.</returns>
        public static float Average(Queue<UdpValue> queue)
        {
            return queue.Count == 0 ? 0 : queue.Sum(q => q.Value)/queue.Count;
        }
    }
}