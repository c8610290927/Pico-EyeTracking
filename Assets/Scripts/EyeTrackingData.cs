using DataSync;
using System;

namespace LabData
{
    [Serializable]
    public class EyeTrackingData : LabDataBase
    {
        public float distance;
        public float gameTime;
        public float speed;
        public float winkTimesL;
        public float winkTimesR;
    }

}