using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabData;
namespace TestGameFrame
{
    public class Test_EnemyTask : TaskBase
    {
        public Test_MainScneRes MainScneRes { get; set; }

        public GameObject Enemy { get; set; }

        public Test_EnemyConfig EnemyConfig { get; set; }

        public override IEnumerator TaskInit()
        {
            Enemy = GameEntityManager.Instance.GetCurrentSceneRes<Test_MainScneRes>().Enemy.gameObject;
            EnemyConfig = LabTools.GetConfig<Test_EnemyConfig>();
            yield return null;
        }

        public override IEnumerator TaskStart()
        {
            for (int i = 0; i < EnemyConfig.WaveCont; i++)
            {
                for (int j = 0; j < EnemyConfig.NumberPerWave; j++)
                {
                    var enemy = GameObject.Instantiate(Enemy, new Vector3(Random.Range(EnemyConfig.MinX, EnemyConfig.MaxX), Enemy.transform.position.y, Random.Range(EnemyConfig.MinZ, EnemyConfig.MaxZ)), new Quaternion());
                    yield return new WaitForSeconds(EnemyConfig.IntervalTime);
                }
            }

        }

        public override IEnumerator TaskStop()
        {
            GameEventCenter.DispatchEvent("Result");
            yield return null;
        }
    }
}
