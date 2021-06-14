using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class poolingObject : MonoBehaviour
    {
        // use
        public bool IsUse { get; protected set; }

        // init
        protected void preInit()
        {
            IsUse = true;
            gameObject.SetActive(true);
        }

        // destroy
        protected abstract void Destroy();
        protected void preDestroy()
        {
            IsUse = false;
            gameObject.SetActive(false);
        }
    }
}