using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Udp;

namespace Neurorehab.Scripts.Devices.Data
{
    /// <summary>
    ///  Represents a set of data belonging to a bitalino device id
    /// </summary>
    public class BitalinoData : GenericDeviceData, IInitializeOwnParameters
    {
        public BitalinoData(string id, string deviceName, StringValues values) : base(id, deviceName, values)
        {
        }

        public void InitializeOwnParameters()
        {

        }
    }
}