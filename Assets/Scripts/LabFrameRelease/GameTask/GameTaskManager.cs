using GameData;
using LabData;
using System.Collections;
using System.Collections.Generic;

public class GameTaskManager : MonoSingleton<GameTaskManager>, IGameManager

{
    int IGameManager.Weight => GobalData.GameTaskManagerWeight;

    private List<TaskBase> _queuetasks;

    void IGameManager.ManagerInit()
    {
        _queuetasks = TaskFanctory.GetCurrentScopeTasks();
    }

    IEnumerator IGameManager.ManagerDispose()
    {
        _queuetasks.Clear();
        yield return null;
    }


    public void StartGameTask()
    {
        _queuetasks.ForEach(p => { StartCoroutine(StartGameTaskEnumerator(p)); });

    }



    private IEnumerator StartGameTaskEnumerator(TaskBase taskBase)
    {
        yield return StartCoroutine(taskBase.TaskInit());
        yield return StartCoroutine(taskBase.TaskStart());
        yield return StartCoroutine(taskBase.TaskStop());
    }
}
