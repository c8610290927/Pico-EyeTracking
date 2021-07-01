using System;
using Neurorehab.Scripts.Devices.Abstracts;

namespace Neurorehab.Scripts.Devices
{
    /// <summary>
    /// Resonsible for saving a boolean value and its received timestamp.
    /// <para>Used as the value of the <see cref="GenericDeviceData.BooleanProperties"/> dictionary</para>
    /// </summary>
    public class UdpBoolean
    {
        /// <summary>
        /// The boolean value received
        /// </summary>
        public bool Boolean { get; set; }

        /// <summary>
        /// Las time that the data was updates
        /// </summary>
        public DateTime LastTimeUpdated { get; set; }
    }
}