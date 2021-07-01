using System;
using System.Collections;
using System.Collections.Generic;
using DataSync;
using UnityEngine;

namespace LabData
{
    public class LabBodyData : LabDataBase
    {
        public LabBodyData(float posX, float posY, float posZ)
        {
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
        }

        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }


        

       

       
    }

}

