using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeFixationsManager : MonoBehaviour
{
    Transform _selectObj;
    float Timer = 0f;
    float gameTime = 0f;
    bool timeGate = false;  //注視計時開關
    float fixationsGate = 2f; //注視時間設定

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    void Update()
    {
        print("timer: " + Timer);
        print("Game Time: " + gameTime);
        gameTime += Time.deltaTime;
        
        if (timeGate)
            Timer += Time.deltaTime;
        if(Timer >= fixationsGate)
            Destroy(GameObject.Find(_selectObj.gameObject.name));

        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20))
            {
                print("inside eyetracking ");
                if (hit.collider.transform.name.Equals("line")) return;
                //從注視狀況切換
                if (_selectObj != null && _selectObj != hit.transform)
                {
                    timeGate = false;
                    _selectObj = null;
                }
                //進入注視狀況
                else if (_selectObj == null)  
                {
                    //Timer += Time.deltaTime;
                    Timer = 0f;
                    timeGate = true;
                    _selectObj = hit.transform;
                    //Object.transform.GetComponent<Renderer>().material.color = Color.yellow;
                    _selectObj.GetComponent<Renderer>().material.color = Color.blue;

                }

            }
            else
            {
                //未注視狀況
                if (_selectObj != null)  
                {
                    //Timer = 0f;
                    timeGate = false;
                    _selectObj.GetComponent<Renderer>().material.color = Color.red;
                    //Object.transform.GetComponent<Renderer>().material.color = Color.red;
                    _selectObj = null;
                }

            }
        }
        else
        {
            if (_selectObj)
            {
                _selectObj = null;
            }
        }

    }
}
