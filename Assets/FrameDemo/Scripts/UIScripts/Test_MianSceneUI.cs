using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestGameFrame
{
    public class Test_MianSceneUI : MonoBehaviour
    {
        public Text ScoreText;

        public int Score = 0;

        public Text EatCount;

        public Text PlayTime;

        public GameObject UIObject;

        private Test_ResultData resultData;

        public Button QuitButton;

        private void Awake()
        {
            GameEventCenter.AddEvent("AddScore", UpdateScore);
            GameEventCenter.AddEvent("Result", Result);
        }

        public void Result()
        {
            resultData = new Test_ResultData(Time.time, Score / 10);
            EatCount.text = resultData.EatCount.ToString();
            PlayTime.text = resultData.Timer.ToString();
            UIObject.SetActive(true);
            GameDataManager.LabDataManager.SendData(resultData);
            QuitButton.onClick.AddListener(ButtonAddListener);
        }

        public void UpdateScore()
        {
            ScoreText.text = "分数:" + (Score += 10).ToString();
        }

        public void ButtonAddListener()
        {
            GameApplication.Instance.GameApplicationDispose();
        }
    }
}
