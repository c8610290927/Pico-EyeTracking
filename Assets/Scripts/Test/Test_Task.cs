using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Task : TaskBase
{
    private Test_SrcGet srcGet;

    public override IEnumerator TaskInit()
    {
        srcGet = GameEntityManager.Instance.GetCurrentSceneRes<Test_SrcGet>();
        yield return null;
    }

    public override IEnumerator TaskStart()
    {
        Debug.Log(srcGet.test);
        yield return null;
    }

    public override IEnumerator TaskStop()
    {
        Debug.Log(srcGet.testGameObj.name);
        yield return null;
    }
}
