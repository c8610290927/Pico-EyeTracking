using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameTask
{
    public Queue<TaskBase> SingleTask;
    public bool IsAllTaskCompleted;

    public GameTask()
    {
        SingleTask=new Queue<TaskBase>();
        IsAllTaskCompleted = false;
    }
}
