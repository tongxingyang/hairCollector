using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class destructiveObstacle : spineCtrl, IDamage
    {
        [SerializeField] GameObject _fix;

        float _hp = 20f;

        public void Init()
        {
            _hp = 20f;
            _fix.SetActive(true);
            SetAnimation("idle", true, 0.5f);

            setEvent();
        }

        void setEvent()
        {
            _spine.AnimationState.Event += delegate (TrackEntry trackEntry, Spine.Event e)
            {
                if (e.Data.Name.Equals("endMotion"))
                {
                    SetAnimation("idle", true, 0.5f);
                }
            };
        }

        public bool getDamaged(float f)
        {
            _hp -= f;

            if (_hp <= 0)
            {
                Destroy();
                return true;
            }

            SetAnimation("shake", false, 1f);
            return false;
        }

        void Destroy()
        {
            _fix.SetActive(false);
            
            SetAnimation("break", false, 1f);
        }

        public void getKnock(Vector3 v, float p, float d) { }

        public void setFrozen(float f)
        {
        }
    }
}