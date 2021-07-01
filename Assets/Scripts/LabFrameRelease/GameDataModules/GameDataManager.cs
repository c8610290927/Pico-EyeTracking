using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DataSync;
using GameData;
using LabData;
using Newtonsoft.Json;
using UnityEngine;


public class GameDataManager : MonoSingleton<GameDataManager>, IGameManager
{
    /// <summary>
    /// LabData
    /// </summary>
    public static ILabDataManager LabDataManager { get; set; }

    /// <summary>
    /// 游戏数据
    /// </summary>
    public static GameFlowData FlowData { get; set; }

    int IGameManager.Weight => GobalData.GameDataManagerWeight;

    void IGameManager.ManagerInit()
    {
        LabDataManager = new LabDataManager();
    }

    IEnumerator IGameManager.ManagerDispose()
    {
        LabDataManager.LabDataDispose();
        yield return null;
    }
}
