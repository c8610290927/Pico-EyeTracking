using System;
using System.Collections;
using System.Collections.Generic;
using DataSync;
using LabData;
using UnityEngine;

public class LabResultDataDemo : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(DataCollectTest());
    }

    IEnumerator DataCollectTest()
    {
        yield return new WaitForSeconds(1f);
        LabDataTestComponent.LabDataManager.SendData(new LabResultDemoData("testResultTest01", "testResultTest02"));
        LabDataTestComponent.LabDataManager.SendData(new LabResultDemoData1("testResultTest04", "testResultTest03"));
    }

   
}
