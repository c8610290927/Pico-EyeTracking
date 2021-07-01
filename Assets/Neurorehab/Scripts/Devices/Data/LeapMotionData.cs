using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Udp;

namespace Neurorehab.Scripts.Devices.Data
{
    /// <summary>
    ///  Represents a set of data belonging to a leapmotion device id (a hand)
    /// </summary>
    public class LeapMotionData : GenericDeviceData, IInitializeOwnParameters
    {
        /// <summary>
        /// Tells which hand is (left or right). Used to show the correct hand prefab in the demo
        /// </summary>
        public bool LeftHanded { get; private set; }


        public LeapMotionData(string id, string deviceName, StringValues values) : base(id, deviceName, values)
        {
            InitializeOwnParameters();
        }

        public void InitializeOwnParameters()
        {
            LeftHanded = GetParameter(TrackingTypeParameter.side.ToString()) == "left";
        }

    }
}