using LabData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameData;

public class GameEntityManager : MonoSingleton<GameEntityManager>, IGameManager
{
    int IGameManager.Weight => GobalData.GameEntityManagerWeight;

    public IGameEntityController EntityController { get; private set; }

    public GameSceneEntityRes CurrentSceneRes;

    IEnumerator IGameManager.ManagerDispose()
    {
        EntityController.GameEntities?.ForEach(p => p.EntityDispose());
        EntityController.GameEntities?.Clear();
        EntityController.GameEntities = null;
        yield return null;
    }

    void IGameManager.ManagerInit()
    {
        EntityController = GetComponent<GameEntityController>();
        EntityController.GameEntities = new List<GameEntityBase>();
    }

    public void SetSceneEntity()
    {
        CurrentSceneRes = FindObjectOfType<GameSceneEntityRes>();
    }

    public T GetCurrentSceneRes<T>()where T: GameSceneEntityRes
    {
        if (CurrentSceneRes != null && CurrentSceneRes is T variable)
        {
            return variable;
        }
        else
        {
            return null;
        }
    }



}
