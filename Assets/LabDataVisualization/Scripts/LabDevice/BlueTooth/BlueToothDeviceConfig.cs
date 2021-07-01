using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabDevice
{
    [Serializable]
    public class BlueToothDeviceConfig
    {
       
        public string BluetoothAddress { get; set; }

        public string BluetoothPin { get; set; }

        public string DataFormat { get; set; }

        public BluetoothGetType BluetoothGetType { get; set; }

        public BlueToothDeviceConfig()
        {
            BluetoothGetType = BluetoothGetType.Read;
            BluetoothAddress = "C33C01059356";
            BluetoothPin = "0000";
            DataFormat = "0x{0:X2}";
        }
    }
}
