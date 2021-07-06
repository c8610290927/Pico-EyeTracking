using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSync;
using System;

namespace LabData
{
    [Serializable]
    public class EyePositionData : LabDataBase
    {
        public float positionX;
        public float positionY;
        public float positionZ;
        public float leftEyeOpenness;
        public float rightEyeOpenness;
        //public Time gameTime;

    }
}