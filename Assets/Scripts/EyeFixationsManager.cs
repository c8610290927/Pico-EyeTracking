using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeFixationsManager : MonoBehaviour
{
    Transform _selectObj;
    float Timer = 0f;
    float gameTime = 0f;
    bool timeGate = false;
    //public Text time;

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    void Update()
    {
        print("timer: " + Timer);
        print("Game Time: " + gameTime);
        gameTime += Time.deltaTime;
        if (timeGate)
        {
            Timer += Time.deltaTime;
        }
        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20))
            {
                print("inside eyetracking ");
                if (hit.collider.transform.name.Equals("line")) return;
                if (_selectObj != null && _selectObj != hit.transform)
                {
                    timeGate = false;
                    //_selectObj.GetComponent<ETMEnity>().StopAnimation();
                    _selectObj = null;
                }
                else if (_selectObj == null)  //注視狀況下
                {
                    //Timer += Time.deltaTime;
                    timeGate = true;
                    _selectObj = hit.transform;
                    //_selectObj.GetComponent<ETMEnity>().PlayAnimation();
                    //Object.transform.GetComponent<Renderer>().material.color = Color.yellow;
                    _selectObj.GetComponent<Renderer>().material.color = Color.blue;
                    //Destroy(GameObject.Find(_selectObj.gameObject.name));

                }

            }
            else
            {
                if (_selectObj != null)  //未注視狀況下
                {
                    //Timer = 0f;
                    timeGate = false;
                    _selectObj.GetComponent<Renderer>().material.color = Color.red;
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
