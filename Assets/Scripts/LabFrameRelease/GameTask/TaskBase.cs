using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskBase 
{
    public abstract IEnumerator TaskInit();
    public abstract IEnumerator TaskStart();
    public abstract IEnumerator TaskStop();

}
