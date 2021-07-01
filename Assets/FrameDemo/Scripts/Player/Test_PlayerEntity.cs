using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGameFrame
{
    public class Test_PlayerEntity : GameEntityBase
    {
        private float speed = 5;

        public override void EntityDispose()
        {

        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                other.gameObject.GetComponent<GameEntityBase>().EntityDestroy();
                GameEventCenter.DispatchEvent("AddScore");
            }
        }


        private void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            this.gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + new Vector3(h, 0, v) * speed * Time.deltaTime);
        }
    }
}
