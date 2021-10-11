using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameData;

public class MainMenuController : MonoBehaviour
{
    //[SerializeField] private Text _userID;

    private void Start()
    {
        print("[create] game start");
        #region Game Config
        GameFlowData gameFlow = new GameFlowData();

        //gameFlow.UserId = _userID.text + "_" + GetRandomID();
        gameFlow.UserId = "test_" + GetRandomID();

        GameDataManager.FlowData = gameFlow;

        #endregion

        gameFlow.UserId = "ouo";

        // New LabData module
        if (GameDataManager.LabDataClient == null || !GameDataManager.LabDataClient.IsClintInit)
        {
            GameDataManager.LabDataClient = new ClientDataSyncLink();
            GameDataManager.LabDataClient.SyncClientInit<Mindfrog.ChildClass.ChildClassScopeInput>("ChildClassConfig");
        }
        GameDataManager.LabDataClient.TaskScopeInit();


        Debug.Log("[create]: MainMenuController  LabDataCollectInit Start=======");
        GameDataManager.LabDataManager.LabDataCollectInit(() => gameFlow.UserId);
        GameDataManager.LabDataManager.SendData(gameFlow);
        GameDataManager.LabDataClient.Scope.Send(gameFlow);
        print("[create]ID: " + gameFlow.UserId);

        Debug.Log("[create]: MainMenuController  Change2MainScene Start=======");
        GameSceneManager.Instance.Change2MainScene();

    }

    //隨機生成UserID
    private System.String GetRandomID()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new System.String(stringChars);
    }

}
