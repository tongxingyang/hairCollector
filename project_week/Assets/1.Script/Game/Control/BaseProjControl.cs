using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class BaseSkillControl : poolingObject
    {
        protected float _dmg;
        protected float _speed;


        public override void Destroy()
        {
            preDestroy();

            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}