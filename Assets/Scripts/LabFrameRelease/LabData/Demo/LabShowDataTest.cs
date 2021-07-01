using System.Collections;
using System.Collections.Generic;
using DataSync;
using LabData;
using UnityEngine;

public class LabShowDataTest : MonoBehaviour
{
    // Start is called before the first frame update

    private LabBodyData _labBodyData;
    void Start()
    {
        LabDataTestComponent.LabDataManager.GetDataAction += (a) => _labBodyData = LabTools.GetData<LabBodyData>(a);
    }

    void Update()
    {
        Debug.Log(_labBodyData?.PosX);
    }
}
