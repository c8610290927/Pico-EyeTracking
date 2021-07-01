using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SerializeField]
public class ApplicationConfig 
{
    public bool IsOpenVR { get; set; }

    public bool OneSelf { get; set; }

    public ApplicationConfig()
    {
        IsOpenVR = false;
        OneSelf = true;
    }
}
