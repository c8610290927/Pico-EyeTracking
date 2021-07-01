using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGameFrame
{
    public class Test_EnemyConfig
    {
        public int WaveCont { get; set; }

        public int NumberPerWave { get; set; }

        public float IntervalTime { get; set; }

        public int MaxZ { get; set; }
        public int MinZ { get; set; }
        public int MaxX { get; set; }
        public int MinX { get; set; }

        public Test_EnemyConfig()
        {
            WaveCont = 3;
            NumberPerWave = 10;
            IntervalTime = 10;
            MaxZ = 13;
            MinZ = -13;
            MaxX = 8;
            MinX = -8;
        }

    }
}
