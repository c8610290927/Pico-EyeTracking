using GameData;
using LabData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoSingleton<GameSceneManager>, IGameManager
{
    private AsyncOperation _operation;

    public Scene CurrentScene { get; private set; }

    int IGameManager.Weight => GobalData.GameSceneManagerWeight;


    /// <summary>
    /// 加载场景传入Action为加载完成后需要执行的Func
    /// </summary>
    /// <param name="actions"></param>
    /// <param name="sceneName"></param>
    public void ChangeScene(List<Action> actions, string sceneName = null, LoadSceneMode mode = LoadSceneMode.Single)
    {
        //场景名
        _operation = null;
        _operation = SceneManager.LoadSceneAsync(sceneName, mode);
        _operation.completed += (AsyncOperation obj) =>
        {
            OnSceneChangeCompleted();
            actions?.ForEach(p => p.Invoke());
        };
        _operation.allowSceneActivation = true;
    }


    /// <summary>
    /// 加载后做的事儿
    /// </summary>
    private void OnSceneChangeCompleted()
    {
        CurrentScene = SceneManager.GetActiveScene();
        Debug.Log(CurrentScene);
    }

    /// <summary>
    /// 跳转到UI场景
    /// </summary>
    public void Change2MainUI()
    {
        ChangeScene(new List<Action>()
        {
            GameUIManager.Instance.StartMainUiLogic
            //TODO 转场UI需要做的事情
        }, GobalData.MainUiScene);
    }

    /// <summary>
    /// 跳转到主场景
    /// </summary>
    public void Change2MainScene()
    {
        ChangeScene(new List<Action>()
        {
            
            GameEntityManager.Instance.SetSceneEntity,
            GameTaskManager.Instance.StartGameTask,
            
            //TODO 转场出场景需要做的事情
        }, GobalData.MainScene);
    }

    void IGameManager.ManagerInit()
    {
        
    }

    IEnumerator IGameManager.ManagerDispose()
    {
        yield return null;
    }
}
