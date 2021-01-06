using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class poolingObject : MonoBehaviour, IPause
    {
        // use
        protected bool _isUse;
        public bool IsUse { get => _isUse; set => _isUse = value; }

        // init
        protected void preInit()
        {
            _isUse = true;
            gameObject.SetActive(true);
        }

        // destroy
        public abstract void Destroy();
        protected void preDestroy()
        {
            _isUse = false;
            gameObject.SetActive(false);
        }

        // pause
        public abstract void onPause(bool bl);
    }
}