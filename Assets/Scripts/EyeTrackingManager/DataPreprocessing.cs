using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using LabData;
using TensorFlowLite;


public class DataPreprocessing : MonoBehaviour
{
    Interpreter interpreter;
    float[] inputs = new float[5];
    float[] outputs = new float[1];

    // Start is called before the first frame update
    void Start()
    {
        //資料前處理
        DataPreprocess();

        //開始load模型
        MLmodelLoad();
    }

    public void DataPreprocess()
    {
        string line;
        string[] line2, line3;
        bool eyePositionGate = true;
        bool winkGateL = true, winkGateR = true;
        float gameTimeNow = 0, gameTime = 0;
        double dist = 0;
        int winkTimesR = 0, winkTimesL = 0;
        int count = 0;
        List<float> X = new List<float>();
        List<float> Y = new List<float>();

        print("game:他有進dataPreprocessing啦啦啦啦啦:(");
        print("game path: " + DebugHelper.path);

        //讀檔後開始進行前處理
        StreamReader sr = new StreamReader(DebugHelper.path); //實際狀況
                                                              //StreamReader sr = new StreamReader("C:\\Users\\pocky chang\\testData\\N4_saccades.txt"); //電腦測試
                                                              //StreamReader sr = new StreamReader("\\storage\\emulated\\0\\Android\\data\\com.DefaultCompany.PicoSDKTEST\\files\\202108291835saccades.txt"); //pico測試

        print("game:現在是檔案測試:(");

        //Read the first line of text
        line = sr.ReadLine();
        //Continue to read until you reach end of file
        while (line != null)
        {
            //write the lie to console window
            //print(line);
            line2 = line.Split(']');
            line3 = line2[1].Split(':');
            if (line3.Length > 1)
            {
                //print("切出來的東西: "+line3[0]+", "+line3[1]);

                if (eyePositionGate) //如果遊戲時間間隔大於0.1秒就記錄眼動位置(擔心數據太多 *10比較好畫圖)
                {
                    if (line3[0] == "eyeTracking (X) (vector)")
                    {
                        if (float.Parse(line3[1]) * 10 != 0.0)
                            X.Add(float.Parse(line3[1]) * 10);
                    }

                    else if (line3[0] == "eyeTracking (Y) (vector)")
                    {
                        if (float.Parse(line3[1]) * 10 != 0.0)
                            Y.Add(float.Parse(line3[1]) * 10);
                        eyePositionGate = false; //眼動位置紀錄完記得把開關關掉
                    }

                }
                if (line3[0] == "game time")
                {
                    gameTimeNow = float.Parse(line3[1]); //把現在的遊戲時間記下來
                    if (gameTimeNow - gameTime >= 0.05) //將現在遊戲時間與上次紀錄眼動參數的時間相減 找個間格時間當輸出條件
                    {
                        gameTime = gameTimeNow; //更新眼動紀錄時間
                        eyePositionGate = true; //眼動位置紀錄開關打開
                    }
                }

                if (line3[0] == "openness (Left)") //記得0之前要加個空白QQ
                {
                    if (line3[1] == " 0")
                        winkGateL = false; //閉上眼睛變成False
                    else if (!winkGateL && line3[1] == " 1")
                    {
                        winkGateL = true;
                        winkTimesL = winkTimesL + 1;
                    }
                }
                else if (line3[0] == "openness (Right)")
                {
                    if (line3[1] == " 0")
                        winkGateR = false; //閉上眼睛變成False
                    else if (!winkGateR && line3[1] == " 1")
                    {
                        winkGateR = true;
                        winkTimesR = winkTimesR + 1;
                    }
                }

            }
            //Read the next line
            line = sr.ReadLine();

        }

        //計算眼動總距離與速度
        while (true)
        {
            dist = dist + Math.Sqrt(Math.Pow(X[count] - X[count + 1], 2) + Math.Pow(Y[count] - Y[count + 1], 2));
            if (count != X.Count - 2)
                count = count + 1;
            else
                break;
        }
        inputs[0] = (float)dist;
        inputs[1] = gameTimeNow;
        inputs[2] = (float)(dist / gameTimeNow);
        inputs[3] = winkTimesL;
        inputs[4] = winkTimesR;
        for (int i = 0; i < 5; i++)
            print("game inputs " + i + ": " + inputs[i]);

        //close the file
        sr.Close();
        print("game: 前處理結束-u-");

        //回傳labdata的資料 要另外寫一個class
        /*EyeTrackingData eyetrackingdata = new EyeTrackingData() //記錄eyedata
        {
            distance = inputs[0],
            gameTime = inputs[1],
            speed = inputs[2],
            winkTimesL = inputs[3],
            winkTimesR = inputs[4]
        };

        //回傳labdata出去
        GameDataManager.LabDataManager.SendData(eyetrackingdata);
        Invoke("EndGame", 10f);*/
    }

    private void MLmodelLoad()
    {
        print("game: 現在在MLmodelLoad裡面喔");
        // NO GPU
        var options = new InterpreterOptions()
        {
            threads = 2
        };
        // 读取TFLite Model文件
        print("game: 讀ML file囉");
        //interpreter = new Interpreter(File.ReadAllBytes("C:\\Users\\pocky chang\\Saccades_NN_tflite_model.tflite"));
        //interpreter = new Interpreter(FileUtil.LoadFile("C:\\Users\\pocky chang\\Saccades_NN_tflite_model.tflite"));
        interpreter = new Interpreter(FileUtil.LoadFile("/storage/emulated/0/Android/data/com.DefaultCompany.PicoSDKTEST/files/Saccades_NN_tflite_model.tflite"),options);
        print("game: 讀完囉:D");

        var inputInfo = interpreter.GetInputTensorInfo(0);
        var outputInfo = interpreter.GetOutputTensorInfo(0);

        //print(inputInfo.shape); //shape[1]=5
        //print(outputInfo.shape);//shape[1]=1
        // 分配输入缓冲区
        interpreter.ResizeInputTensor(0, inputInfo.shape);
        interpreter.AllocateTensors();
        // 传入输入数据
        interpreter.SetInputTensorData(0, inputs);
        // 執行
        interpreter.Invoke();

        // 獲取輸出數據
        interpreter.GetOutputTensorData(0, outputs);

        print("game outputs: " + outputs[0]);

        Invoke("EndGame", 3f);
    }
    void OnDestroy()
    {
        interpreter?.Dispose();
    }

    private void EndGame()
    {
        Application.Quit();
    }
}
