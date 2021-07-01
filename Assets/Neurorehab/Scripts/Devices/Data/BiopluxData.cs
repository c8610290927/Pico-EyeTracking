using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Udp;

namespace Neurorehab.Scripts.Devices.Data
{
    /// <summary>
    ///  Represents a set of data belonging to a bioplux device id
    /// </summary>
    public class BiopluxData : GenericDeviceData, IInitializeOwnParameters
    {
        public BiopluxData(string id, string deviceName, StringValues values) : base(id, deviceName, values)
        {
        }

        public void InitializeOwnParameters()
        {

        }
    }
}