using UnityEngine;

namespace Neurorehab.Scripts.Devices.Abstracts
{
    /// <summary>
    /// Base class for the Unity Game Object that represets a set of data from a <see cref="Abstracts.GenericDeviceData"/>.
    /// </summary>
    public class GenericDeviceUnity : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Abstracts.GenericDeviceData"/> that contains the Data to be used by this object.
        /// </summary>
        public GenericDeviceData GenericDeviceData { get; set; }

        /// <summary>
        /// Sets the <see cref="GenericDeviceData"/> as the device received in the parameters.
        /// </summary>
        /// <param name="device">The <see cref="Abstracts.GenericDeviceData"/> that contains the Data to be used by this object.</param>
        public void SetDeviceData(GenericDeviceData device)
        {
            GenericDeviceData = device;
        }
    }
}