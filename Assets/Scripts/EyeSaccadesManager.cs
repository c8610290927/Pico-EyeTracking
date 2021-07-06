using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;
using UnityEngine.UI;
using LabData;

public class EyeSaccadesManager : MonoBehaviour
{
    Transform _selectObj;
    float gameTime = 0f;  //遊戲時間
    int saccadeTime = 2;  //掃視需求次數(可設定)
    int count = 0;  //掃視消滅總數
    public GameObject dirtyThing;
    public GameObject gameoverCanves;  //遊戲結束視窗

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    Pvr_UnitySDKAPI.EyeTrackingData eyeTrackingData = new Pvr_UnitySDKAPI.EyeTrackingData();
    
    void Update()
    {
        gameTime += Time.deltaTime;
        //print("Game Time: " + gameTime);
        //print("eyeTrackingData (openess): "+eyeTrackingData.rightEyeOpenness);
        
        if (count == 40)
        {
            count = 0;
            if (saccadeTime != 0)
                Instantiate(dirtyThing, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            else
                gameoverCanves.SetActive(true);
                //輸出遊戲時間那些東西的數據
        } 
        else 
        {
            gameTime += Time.deltaTime;
        }

        bool result_data = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingData(ref eyeTrackingData);
        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 20))
            {
                if (hit.collider.transform.name.Equals("line")) return;
                print("hit: "+ hit.transform.position);
                if (_selectObj != null && _selectObj != hit.transform)
                {
                    _selectObj = null;
                }
                else if (_selectObj == null)  //注視狀況下
                {
                    if (count != 40)
                    {
                        _selectObj = hit.transform;
                        Destroy(GameObject.Find(_selectObj.gameObject.name));
                        count = count + 1;
                    }
                    if (count == 40)
                        saccadeTime = saccadeTime - 1;
                }

            }
            else
            {
                if (_selectObj != null)  //未注視狀況下
                {
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
                if(count!=40 || saccadeTime!=0)
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
