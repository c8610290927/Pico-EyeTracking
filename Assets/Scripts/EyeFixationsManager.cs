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
    int fixationTime = 9;  //注視需求次數-1
    bool gameTimeGate = true;  //遊戲計時開關(true:開啟計時)
    //生成20組注視物件位置
    float[] positionX = new float[] {-0.47f, 1.75f, 0.84f, -0.95f, -2.63f, 0.85f, 2.16f, -1.74f, 1.01f, -2.50f, 2.72f, 1.03f, -0.52f, 2.67f, -1.26f, -1.39f, -2.97f, 2.95f, 1.59f, -1.60f};
    float[] positionY = new float[] {0.80f, -0.01f, 1.54f, -0.04f, -0.50f, 0.63f, 0.86f, 0.14f, 1.07f, 1.80f, -0.14f, 1.22f, 0.10f, 1.70f, 1.63f, 1.87f, 0.94f, 1.41f, -0.41f, 1.70f};

    Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    Pvr_UnitySDKAPI.EyeTrackingData eyeTrackingData = new Pvr_UnitySDKAPI.EyeTrackingData();
    
    void Update()
    {
        //print("timer: " + Timer);
        //print("Game Time: " + gameTime);
        if (gameTimeGate)
            gameTime += Time.deltaTime;
        
        if (timeGate)
            Timer += Time.deltaTime;

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
                    {
                        gameoverCanves.SetActive(true);
                        gameTimeGate = false;  //關閉遊戲計時器
                        //輸出遊戲數據 (進入前處理+ML分析)
                        print("GameOver");
                        //10秒後自動關閉遊戲
                        Invoke("EndGame", 10f);
                    }
                        
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
            if(result_data && gameTimeGate)
            {
                print("game time: "+gameTime);
                print("openness (Right): "+eyeTrackingData.rightEyeOpenness);
                print("openness (Left): "+eyeTrackingData.leftEyeOpenness);
                print("eyeTracking (X) (vector): "+ eyeTrackingData.combinedEyeGazeVector.x);
                print("eyeTracking (Y) (vector): "+ eyeTrackingData.combinedEyeGazeVector.y);
                print("eyeTracking (Z) (vector): "+ eyeTrackingData.combinedEyeGazeVector.z);

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
        if (eyepositiondata.timeStamp != 0.0)
            GameDataManager.LabDataManager.SendData(eyepositiondata);

    }

    private void EndGame()
    {
        Application.Quit();
    }
}
