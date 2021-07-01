using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public interface ITask
{
    /// <summary>
    /// 装栈操作，用来存放任务
    /// </summary>
    void QueueEnqueue();

    /// <summary>
    /// 任务开始
    /// </summary>
    /// <returns></returns>
    IEnumerator StartIETask();

    /// <summary>
    /// 任务结束需要做的事件
    /// </summary>
    /// <param name="actions"></param>
    IEnumerator GameEnd(List<Action> actions);

    /// <summary>
    /// 终止协程
    /// </summary>
    void StopIEnumeratorTask();

    /// <summary>
    /// 开始任务前需要做的事情
    /// </summary>
    IEnumerator StartGameInit(List<Action> Inits);


}
