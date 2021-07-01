using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGameFrame
{

    public class Test_PlayerTask : TaskBase
    {
        public Test_MainScneRes MainScneRes { get; set; }

        public GameObject Player { get; set; }

        public override IEnumerator TaskInit()
        {
            MainScneRes = GameEntityManager.Instance.GetCurrentSceneRes<Test_MainScneRes>();
            yield return null;
        }

        public override IEnumerator TaskStart()
        {
            Player = GameObject.Instantiate(MainScneRes.Player.gameObject);

            yield return null;
        }

        public override IEnumerator TaskStop()
        {
            yield return null;
        }
    }
}
