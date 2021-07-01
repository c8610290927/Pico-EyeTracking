using System.Collections;
using System.Collections.Generic;
using DataSync;
using UnityEngine;

namespace LabData
{
    public class LabEyeTracingData : LabDataBase
    {
        public Pos EyePos { get; private set; }

        public LabEyeTracingData(Pos pos)
        {
            EyePos = pos;
        }

    }
}

