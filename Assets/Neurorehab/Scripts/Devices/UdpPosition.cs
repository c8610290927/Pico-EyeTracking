using System;
using Neurorehab.Scripts.Devices.Abstracts;
using UnityEngine;

namespace Neurorehab.Scripts.Devices
{
    /// <summary>
    /// Resonsible for saving a Vector3 value and its received timestamp.
    /// <para>Used as the value of the <see cref="GenericDeviceData.PositionQueues"/> dictionary</para>
    /// </summary>
    public class UdpPosition
    {
        /// <summary>
        /// The position value received
        /// </summary>
        public Vector3 Position { get; set; }


        /// <summary>
        /// Las time that the data was updates
        /// </summary>
        public DateTime LastTimeUpdated { get; set; }
    }
}