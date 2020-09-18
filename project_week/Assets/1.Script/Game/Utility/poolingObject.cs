using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class poolingObject : MonoBehaviour, IPause
    {
        protected bool _isUse;
        public bool IsUse { get => _isUse; set => _isUse = value; }

        protected void preInit()
        {
            _isUse = true;
            gameObject.SetActive(true);
        }

        public abstract void Destroy();
        protected void preDestroy()
        {
            _isUse = false;
            gameObject.SetActive(false);
        }

        public abstract void onPause(bool bl);
    }
}