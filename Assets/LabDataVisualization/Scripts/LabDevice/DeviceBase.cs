using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabDevice
{
    public enum DeviceType
    {
        BlueTooth
    }
    public class DeviceBase
    {
        public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
    }

}

