using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class EnSkillBase : poolingObject, IPause
    {
        protected float _dmg;
        protected float _speed;

        protected override void Destroy()
        {
            preDestroy();
        }

        /// <summary> 일시정지 </summary>
        public abstract void onPause(bool bl);
    }
}