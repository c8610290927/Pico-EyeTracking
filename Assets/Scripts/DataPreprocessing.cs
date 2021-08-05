using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class DataPreprocessing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

        //讀檔後開始進行前處理
        StreamReader sr = new StreamReader("C:\\Users\\pocky chang\\testData\\N1_saccades.txt");
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

                if(eyePositionGate) //如果遊戲時間間隔大於0.1秒就記錄眼動位置(擔心數據太多 *10比較好畫圖)
                {
                    if (line3[0] == "eyeTracking (X) (vector)")
                    {
                        if(float.Parse(line3[1])*10 != 0.0)
                            X.Add(float.Parse(line3[1])*10);
                    }
                        
                    else if(line3[0] == "eyeTracking (Y) (vector)")
                    {
                        if(float.Parse(line3[1])*10 != 0.0)
                            Y.Add(float.Parse(line3[1])*10);
                        eyePositionGate = false; //眼動位置紀錄完記得把開關關掉
                    }
                        
                }
                if (line3[0] == "game time")
                {
                    gameTimeNow = float.Parse(line3[1]); //把現在的遊戲時間記下來
                    if (gameTimeNow-gameTime >= 0.05) //將現在遊戲時間與上次紀錄眼動參數的時間相減 找個間格時間當輸出條件
                    {
                        gameTime = gameTimeNow; //更新眼動紀錄時間
                        eyePositionGate = true; //眼動位置紀錄開關打開
                    }
                }

                if (line3[0] == "openness (Left)") //記得0之前要加個空白QQ
                {
                    if(line3[1] == " 0")
                        winkGateL = false; //閉上眼睛變成False
                    else if(!winkGateL && line3[1] == " 1")
                    {
                        winkGateL = true;
                        winkTimesL = winkTimesL+1;
                    }
                }
                else if (line3[0] == "openness (Right)")
                {
                    if(line3[1] == " 0")
                        winkGateR = false; //閉上眼睛變成False
                    else if(!winkGateR && line3[1] == " 1")
                    {
                        winkGateR = true;
                        winkTimesR = winkTimesR+1;
                    }
                }
                
            }
            //Read the next line
            line = sr.ReadLine();
        }
        
        //計算眼動總距離與速度
        while(true)
        {
            dist = dist + Math.Sqrt(Math.Pow(X[count]-X[count+1], 2)+Math.Pow(Y[count]-Y[count+1], 2));
            if (count != X.Count-2)
                count=count+1;
            else
                break;
        }
        print("dist: "+ dist);
        print("gameTime(total): "+gameTimeNow);
        print("speed: "+ dist/gameTimeNow);
        print("winkTimes (L): "+ winkTimesL);
        print("winkTimes (R): "+ winkTimesR);
        //close the file
        sr.Close();

        //開始load模型
        
    }

}
