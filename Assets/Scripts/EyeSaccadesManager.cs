using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LabData;

public class EyeSaccadesManager : MonoBehaviour
{
    Transform _selectObj;
    float gameTime = 0f;  //遊戲時間
    int saccadeTime = 1;  //掃視需求次數(可設定)
    int count = 0;  //掃視消滅總數
    bool gameTimeGate = true;  //遊戲計時開關(true:開啟計時)
    public GameObject dirtyThing;
    public GameObject gameoverCanves;  //遊戲結束視窗

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    Pvr_UnitySDKAPI.EyeTrackingData eyeTrackingData = new Pvr_UnitySDKAPI.EyeTrackingData();

    void Update()
    {
        //SceneManager.LoadScene("DataAnalysis");
        if (gameTimeGate)  //開關沒關 持續計時
            gameTime += Time.deltaTime;
        
        if (count == 40) //把所有東西都削掉了
        {
            count = 0;
            if (saccadeTime != 0)
                Instantiate(dirtyThing, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            else
            {
                gameoverCanves.SetActive(true);
                gameTimeGate = false;
                print("[create] GameOver");
                //輸出遊戲數據 (進入前處理+ML分析)
                //gameObject.GetComponent<DataPreprocessing>().DataPreprocess();
                //20秒後自動關閉遊戲
                Invoke("EndGame", 20f);

            }
                
        } 

        bool result_data = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingData(ref eyeTrackingData);
        bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        //回傳labdata的資料 要另外寫一個class (記錄eyedata)
        EyePositionData eyepositiondata = new EyePositionData();
        if (result)
        {
            Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 20))
            {
                if (hit.collider.transform.name.Equals("line")) return;
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
            if(result_data && gameTimeGate)
            {
                print("[create] game time: " + gameTime);
                print("openness (Right): "+eyeTrackingData.rightEyeOpenness);
                print("openness (Left): "+eyeTrackingData.leftEyeOpenness);
                //print("eyeTracking (Right) (pupil): "+eyeTrackingData.rightEyePupilDilation);
                //print("eyeTracking (Left) (pupil): "+eyeTrackingData.leftEyePupilDilation);
                
                print("eyeTracking (X) (vector): " + eyeTrackingData.combinedEyeGazeVector.x);
                print("eyeTracking (Y) (vector): " + eyeTrackingData.combinedEyeGazeVector.y);
                print("eyeTracking (Z) (vector): " + eyeTrackingData.combinedEyeGazeVector.z);


                eyepositiondata.timeStamp = gameTime;
                eyepositiondata.positionX = eyeTrackingData.combinedEyeGazeVector.x;
                eyepositiondata.positionY = eyeTrackingData.combinedEyeGazeVector.y;
                eyepositiondata.positionZ = eyeTrackingData.combinedEyeGazeVector.z;
                eyepositiondata.leftEyeOpenness = eyeTrackingData.leftEyeOpenness;
                eyepositiondata.rightEyeOpenness = eyeTrackingData.rightEyeOpenness;
                
            }
        }
        else
        {
            if (_selectObj)
            {
                _selectObj = null;
            }
        }
        //回傳labdata出去
        if(eyepositiondata.timeStamp != 0.0)
            GameDataManager.LabDataManager.SendData(eyepositiondata);

    }

    private void EndGame()
    {
        Application.Quit();
    }
}
