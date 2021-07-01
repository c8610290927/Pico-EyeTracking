using System.Collections;
using System.Collections.Generic;
using LabData;
using UnityEngine;

public class LabDataTestComponent : MonoBehaviour
{
    public static  ILabDataManager LabDataManager { get; set; }
    // Start is called before the first frame update

    void Awake()
    {
        LabDataManager = new LabDataManager();
    }

    public void OnDisable()
    {
        LabDataManager.LabDataDispose();
    }


}
