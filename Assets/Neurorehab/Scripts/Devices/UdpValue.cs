using System;
using Neurorehab.Scripts.Devices.Abstracts;

namespace Neurorehab.Scripts.Devices
{
    /// <summary>
    /// Resonsible for saving a float value and its received timestamp.
    /// <para>Used as the value of the <see cref="GenericDeviceData.FloatQueues"/> dictionary</para>
    /// </summary>
    public class UdpValue
    {
        /// <summary>
        /// The float value received
        /// </summary>
        public float Value { get; set; }


        /// <summary>
        /// Las time that the data was updates
        /// </summary>
        public DateTime LastTimeUpdated { get; set; }

        public UdpValue()
        {
            Value = 0;
            LastTimeUpdated = DateTime.Now;
        }
    }
}