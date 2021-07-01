using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabData;
using GameData;
using UnityEngine.UI;

namespace TestGameFrame
{
    public class Test_MenuUI : MonoBehaviour
    {
        public Button SatrtButton;

        public void Start()
        {
            SatrtButton.onClick.AddListener(StartButtonClick);
        }

        public void StartButtonClick()
        {
            GameFlowData gameFlow = new GameFlowData();

            GameDataManager.FlowData = gameFlow;

            var Id = gameFlow.UserId;

            GameDataManager.LabDataManager.LabDataCollectInit(() => Id);

            GameSceneManager.Instance.Change2MainScene();
        }
    }
}
