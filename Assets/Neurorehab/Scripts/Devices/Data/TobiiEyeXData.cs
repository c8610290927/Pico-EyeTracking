using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Udp;

namespace Neurorehab.Scripts.Devices.Data
{
    /// <summary>
    ///  Represents a set of data belonging to a tobiieyex device id
    /// </summary>
    public class TobiiEyeXData : GenericDeviceData, IInitializeOwnParameters
    {
        public TobiiEyeXData(string id, string deviceName, StringValues values) : base(id, deviceName, values)
        {
        }

        public void InitializeOwnParameters()
        {
        }
    }
}