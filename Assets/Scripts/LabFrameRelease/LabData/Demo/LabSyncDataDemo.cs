using DataSync;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LabData
{
    public class LabSyncDataDemo : MonoBehaviour
    {
        public string Id = "Test01";

        private int _i = 1;

        // Use this for initialization
        void Start()
        {
            LabDataTestComponent.LabDataManager.LabDataCollectInit(() => Id);

            DeviceThreadTest deviceThreadTest = new DeviceThreadTest();
            deviceThreadTest.StartTest();        
        }
    }
}

