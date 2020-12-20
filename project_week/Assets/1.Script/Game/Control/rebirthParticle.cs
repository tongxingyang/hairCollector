using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class rebirthParticle : MonoBehaviour
    {
        Action _reAct;
        Action _endAct;

        public void setAct(Action reAct, Action endAct)
        {
            _reAct = reAct;
            _endAct = endAct;
        }

        public void rebirth()
        {
            _reAct?.Invoke();
        }

        public void endAni()
        {
            _endAct?.Invoke();            

            gameObject.SetActive(false);
        }
    }
}