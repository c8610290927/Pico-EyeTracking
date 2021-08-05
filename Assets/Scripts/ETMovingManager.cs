﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;
using UnityEngine.UI;

public class ETMovingManager : MonoBehaviour
{
    Transform _selectObj;
    float gameTime = 0f;  //遊戲時間
    int saccadeTime = 1;  //掃視需求次數
    int count = 0;  //掃視消滅總數
    public GameObject dirtyThing;

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    void Update()
    {
        if (count == 40)
            Instantiate(dirtyThing, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20))
            {
                if (hit.collider.transform.name.Equals("line")) return;
                if (_selectObj != null && _selectObj != hit.transform)
                {
                    //_selectObj.GetComponent<ETMEnity>().StopAnimation();
                    _selectObj = null;
                }
                else if (_selectObj == null)  //注視狀況下
                {
                    _selectObj = hit.transform;
                    //_selectObj.GetComponent<ETMEnity>().PlayAnimation();
                    //Object.transform.GetComponent<Renderer>().material.color = Color.yellow;
                    //_selectObj.GetComponent<Renderer>().material.color = Color.blue;
                    Destroy(GameObject.Find(_selectObj.gameObject.name));
                    count = count + 1;
                }

            }
            else
            {
                if (_selectObj != null)  //未注視狀況下
                {
                    //_selectObj.GetComponent<Renderer>().material.color = Color.red;
                    //Object.transform.GetComponent<Renderer>().material.color = Color.red;
                    //_selectObj.GetComponent<ETMEnity>().StopAnimation();
                    _selectObj = null;
                }

            }
        }
        else
        {
            if (_selectObj)
            {
                //_selectObj.GetComponent<ETMEnity>().StopAnimation();
                _selectObj = null;
            }
        }

    }
}
