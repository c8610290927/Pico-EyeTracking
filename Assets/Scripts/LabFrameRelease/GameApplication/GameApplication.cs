using GameData;
using LabData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class GameApplication : MonoSingleton<GameApplication>
{

    private bool _isOnApplicationQuit;
    /// <summary>
    /// 继承Manager的集合
    /// </summary>
    private List<IGameManager> _gameManagers;


    private void Awake()
    {
        var applicationConfig = LabTools.GetConfig<ApplicationConfig>();
        XRSettings.enabled = applicationConfig.IsOpenVR;
        DontDestroyOnLoad(this);
        GameApplicationInit();       
        //自身启动
        if (applicationConfig.OneSelf)
        {
            OneSelfStart();
        }
        //外部启动
        else
        {
            ExternalStart();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void GameApplicationInit()
    {
        _isOnApplicationQuit = false;
        GameEventCenter.EventCenterInit();
        _gameManagers = FindObjectsOfType<MonoBehaviour>().OfType<IGameManager>().ToList().OrderBy(m => m.Weight).ToList();
        _gameManagers.ForEach(p =>
        {
            p.ManagerInit();          
        });
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void GameApplicationDispose()
    {
        GameEventCenter.RemoveAllEvents();
        StartCoroutine(GameApplicationDisposeEnumerator());
    }

    private IEnumerator GameApplicationDisposeEnumerator()
    {
        if (_gameManagers.Count <= 0)
        {
            yield break;
        }

        for (int i = 0; i < _gameManagers.Count; i++)
        {
            yield return StartCoroutine(_gameManagers[i].ManagerDispose());
        }
        _gameManagers.Clear();
        yield return null;
    }
    public void GameApplicationQuit()
    {
        if (!_isOnApplicationQuit)
        {
            GameApplicationDispose();
            Application.Quit();
            _isOnApplicationQuit = true;
        }
    }

    protected override void OnApplicationQuit()
    {
        GameApplicationQuit();
        base.OnApplicationQuit();
    }

    /// <summary>
    /// 外部启动
    /// </summary>
    private void ExternalStart()
    {
        string[] arguments = Environment.GetCommandLineArgs();

        GameFlowData gameFlowData = new GameFlowData();
        GameDataManager.FlowData = LabTools.GetDataByString<GameFlowData>(gameFlowData.ToJson());
        GameDataManager.LabDataManager.LabDataCollectInit(()=> GameDataManager.FlowData.UserId);
        GameSceneManager.Instance.Change2MainScene();
    }

    /// <summary>
    /// 自身启动
    /// </summary>
    private void OneSelfStart()
    {
        GameSceneManager.Instance.Change2MainUI();
    }


}


