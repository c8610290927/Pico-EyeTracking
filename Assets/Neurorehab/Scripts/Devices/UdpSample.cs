using System;
using System.Collections.Generic;
using Neurorehab.Scripts.Devices.Abstracts;

namespace Neurorehab.Scripts.Devices
{
    /// <summary>
    /// Resonsible for saving a list of float values and its received timestamp.
    /// <para>Used as the value of the <see cref="GenericDeviceData.SampleList"/> dictionary</para>
    /// </summary>
    public class UdpSample
    {
        /// <summary>
        /// The list of float values received
        /// </summary>
        public List<float> Sample { get; set; }

        /// <summary>
        /// Las time that the data was updates
        /// </summary>
        public DateTime LastTimeUpdated { get; set; }
    }
}