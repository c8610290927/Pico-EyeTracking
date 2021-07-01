using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Devices.Data;
using Neurorehab.Scripts.Enums;

namespace Neurorehab.Device_TobiiEyeX.Scripts
{
    /// <summary>
    /// The controller of all the <see cref="TobiiEyeXData"/>. Responsible for creating, deleting and updating all the <see cref="TobiiEyeXData"/> according to what is receiving by UDP.
    /// </summary>
    public class TobiiEyeX : GenericDeviceController
    {
        protected override void Awake()
        {
            base.Awake();

            DeviceName = Devices.tobiieyex.ToString();
        }
    }
}