using System.Linq;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Udp;
using Neurorehab.Scripts.Utilities;
using UnityEngine;

namespace Neurorehab.Scripts.Devices.Data
{
    /// <summary>
    ///  Represents a set of data belonging to a kinect device id
    /// </summary>
    public class KinectData : GenericDeviceData, IInitializeOwnParameters
    {
        /// <summary>
        /// Indicates if this skeleton is the closest one to the kinect
        /// </summary>
        public bool IsMain { get; private set; }

        public KinectData(string id, string deviceName, StringValues values) : base(id, deviceName, values)
        {
            InitializeOwnParameters();
        }

        /// <summary>
        /// Returns the Vector3 representing the position of the given label according to the <see cref="GenericDeviceController.Smoothing"/> and <see cref="GenericDeviceController.PositionMultiplier"/>.
        /// </summary>
        /// <param name="label">The label to get the rotation.</param>
        /// <returns>A Vector3 representing the rotation of the given label where the x and z are multiplied by the <see cref="GenericDeviceController.PositionMultiplier"/>.</returns>
        public override Vector3 GetPosition(string label)
        {
            if (PositionLocks.ContainsKey(label) == false) return Vector3.zero;

            var @lock = PositionLocks[label];
            var queue = PositionQueues[label];

            Vector3 position;

            lock (@lock)
            {
                position = Controller.Smoothing ? 
                    Smoother.Average(queue) : queue.Count == 0 ? 
                    Vector3.zero : queue.Last().Position;
            }

            return new Vector3(position.x * Controller.PositionMultiplier, position.y,
                position.z * Controller.PositionMultiplier);
        }

        public void InitializeOwnParameters()
        {
            IsMain = GetParameter(TrackingTypeParameter.main.ToString()) == "true";
        }
    }
}