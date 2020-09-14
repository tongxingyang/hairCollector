using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class walkDust : poolingObject
    {
        [SerializeField] Animator _ani;

        public void init()
        {
            preInit();
            StartCoroutine(dust());
        }

        IEnumerator dust()
        {
            _ani.SetTrigger("dust");
            yield return new WaitForSeconds(1f);
            Destroy();
        }

        public override void Destroy()
        {
            preDestroy();
        }

        public override void onPause(bool bl)
        {
            _ani.speed = (bl) ? 0f : 1f;
        }
    }
}