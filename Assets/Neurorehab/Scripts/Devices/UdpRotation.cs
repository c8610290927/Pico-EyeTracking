using System;
using Neurorehab.Scripts.Devices.Abstracts;
using UnityEngine;

namespace Neurorehab.Scripts.Devices
{
    /// <summary>
    /// Resonsible for saving a Quaternion value and its received timestamp.
    /// <para>Used as the value of the <see cref="GenericDeviceData.RotationQueues"/> dictionary</para>
    /// </summary>
    public class UdpRotation
    {
        /// <summary>
        /// The rotation value received
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Las time that the data was updates
        /// </summary>
        public DateTime LastTimeUpdated { get; set; }
    }
}