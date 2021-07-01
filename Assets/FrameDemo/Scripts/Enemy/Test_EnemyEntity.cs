using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGameFrame
{
    public class Test_EnemyEntity : GameEntityBase
    {
        public Test_MainScneRes MainScneRes { get; set; }

        public GameObject Player { get; set; }

        public override void EntityDispose()
        {

        }
    }
}
