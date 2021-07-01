using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Udp;

namespace Neurorehab.Scripts.Devices.Data
{
    /// <summary>
    ///  Represents a set of data belonging to a zephyr device id
    /// </summary>
    public class ZephyrData : GenericDeviceData, IInitializeOwnParameters
    {
        public ZephyrData(string id, string deviceName, StringValues values) : base(id, deviceName, values)
        {
        }

        ///// <summary>
        ///// Override of the <see cref="GenericDeviceData"/>. Since Zephyr only sends the axis x rotation value, only that value is passed to the Quaternion.
        ///// </summary>
        ///// <param name="values">The <see cref="StringValues"/> containing the raw data to be parsed.</param>
        //protected override void ProcessRotation(StringValues values)
        //{
        //    SetRotation(values.InformationLabel, Quaternion.Euler(float.Parse(values.Values[0]), 0, 0));
        //}

        public void InitializeOwnParameters()
        {
            
        }
    }
}