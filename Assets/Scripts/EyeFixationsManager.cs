using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using LabData;

public class EyeFixationsManager : MonoBehaviour
{
    public GameObject gameoverCanves;  //遊戲結束視窗
    Transform _selectObj;
    float Timer = 0f;  //注視時間
    float gameTime = 0f;  //遊戲時間
    bool timeGate = false;  //注視計時開關
    float fixationsGate = 2f; //注視時間設定(達到幾秒destroy)
    int count = 0;  //注視成功次數
    int fixationTime = 3;  //注視需求次數
    //生成20組注視物件位置
    float[] positionX = new float[] {-0.47f, 1.75f, 0.84f, -0.95f, -2.63f, 0.85f, 2.16f, -1.74f, 1.01f, -2.50f, 2.72f, 1.03f, -0.52f, 2.67f, -1.26f, -1.39f, -2.97f, 2.95f, 1.59f, -1.60f};
    float[] positionY = new float[] {0.80f, -0.01f, 1.54f, -0.04f, -0.50f, 0.63f, 0.86f, 0.14f, 1.07f, 1.80f, -0.14f, 1.22f, 0.10f, 1.70f, 1.63f, 1.87f, 0.94f, 1.41f, -0.41f, 1.70f};

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    Pvr_UnitySDKAPI.EyeTrackingData eyeTrackingData = new Pvr_UnitySDKAPI.EyeTrackingData();
    
    void Update()
    {
        //print("timer: " + Timer);
        //print("Game Time: " + gameTime);
        if (count != fixationTime)
            gameTime += Time.deltaTime;
        
        if (timeGate)
            Timer += Time.deltaTime;

        bool result_data = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingData(ref eyeTrackingData);
        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20))
            {
                DartBoard dartBoard = hit.transform.GetComponent<DartBoard>();
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
                    //_selectObj.GetComponent<Renderer>().material.color = Color.blue;

                }
                if (Timer >= fixationsGate && hit.transform.tag != "ground")
                {
                    Destroy(GameObject.Find(_selectObj.gameObject.name));
                    
                    if(count != fixationTime)
                    {
                        //Instantiate(dartBoard, new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-0.5f, 2f), 2.5f), new Quaternion(0, 0, 0, 0));
                        Instantiate(dartBoard, new Vector3(positionX[count], positionY[count], 2.5f), new Quaternion(0, 0, 0, 0));
                        _selectObj = null;
                        count = count + 1;
                    }
                    else
                        gameoverCanves.SetActive(true);
                        //輸出遊戲數據
                }

            }
            else
            {
                //未注視狀況
                if (_selectObj != null)  
                {
                    //Timer = 0f;
                    timeGate = false;
                    //_selectObj.GetComponent<Renderer>().material.color = Color.red;
                    _selectObj = null;
                }
            }
            if(result_data)
            {
                print("openness (Right): "+eyeTrackingData.rightEyeOpenness);
                print("openness (Left): "+eyeTrackingData.leftEyeOpenness);
                print("eyeTracking (Right) (pupil): "+eyeTrackingData.rightEyePupilDilation);
                print("eyeTracking (Left) (pupil): "+eyeTrackingData.leftEyePupilDilation);
                
                print("eyeTracking (X) (vector): "+ eyeTrackingData.combinedEyeGazeVector.x);
                print("eyeTracking (Y) (vector): "+ eyeTrackingData.combinedEyeGazeVector.y);
                print("eyeTracking (Z) (vector): "+ eyeTrackingData.combinedEyeGazeVector.z);
                //回傳labdata的資料 要另外寫一個class
                EyePositionData eyepositiondata = new EyePositionData() //記錄eyedata
                {
                    positionX = eyeTrackingData.combinedEyeGazeVector.x,
                    positionY = eyeTrackingData.combinedEyeGazeVector.y,
                    positionZ = eyeTrackingData.combinedEyeGazeVector.z,
                    leftEyeOpenness = eyeTrackingData.leftEyeOpenness,
                    rightEyeOpenness = eyeTrackingData.rightEyeOpenness
                };
                //回傳labdata出去
                if(count != fixationTime)
                    GameDataManager.LabDataManager.SendData(eyepositiondata);
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
