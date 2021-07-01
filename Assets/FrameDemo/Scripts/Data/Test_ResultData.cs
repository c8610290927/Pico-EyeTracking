using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabData;
using DataSync;
namespace TestGameFrame
{
    public class Test_ResultData : LabDataBase
    {
        public float Timer { get; set; }

        public int EatCount { get; set; }

        public Test_ResultData(float timer, int eat)
        {
            Timer = timer;
            EatCount = eat;
        }
    }
}
