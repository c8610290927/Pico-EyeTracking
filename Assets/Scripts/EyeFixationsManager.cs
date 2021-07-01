using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeFixationsManager : MonoBehaviour
{
    Transform _selectObj;
    float Timer = 0f;
    float gameTime = 0f;
    public GameObject Object;
    //public Text time;

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    void Update()
    {
        print("timer: "+Timer);
        print("game time: "+ gameTime);
        gameTime += Time.deltaTime;
        //time.text = Timer.ToString("0.00");
        if(Timer >= 3f)
        {
            Object.transform.GetComponent<Renderer>().material.color = Color.yellow;
        }
        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20))
            {
                print("hit obj: "+ hit.transform.name);
                if (hit.collider.transform.name.Equals("line")) return;
                //馬上更換注視物件
                if (_selectObj != null && _selectObj != hit.transform)
                {
                    Timer = 0f;
                    _selectObj = null;
                }
                //持續注視物件
                else if( _selectObj == hit.transform)
                {
                    Timer += Time.deltaTime;
                    _selectObj.GetComponent<Renderer>().material.color = Color.blue;
                }
                //從未注視轉成注視狀況
                else if (_selectObj == null)  
                {
                    Timer += Time.deltaTime;
                    _selectObj = hit.transform;
                    _selectObj.GetComponent<Renderer>().material.color = Color.blue;
                    //Destroy(GameObject.Find(_selectObj.gameObject.name));
                }

            }
            else
            {
                //注視轉為未注視
                if (_selectObj != null)  
                {
                    Timer = 0f;
                    _selectObj.GetComponent<Renderer>().material.color = Color.red;
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
