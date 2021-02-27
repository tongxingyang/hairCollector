using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

namespace week
{
    public class destructiveObstacle : spineCtrl, IDamage
    {
        [SerializeField] GameObject _fix;

        int _hp = 2;
        public float getHp()
        {
            return _hp;
        }

        public void Init()
        {
            _hp = 2;
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

        public float getDamaged(float val, bool ignoreDef = false)
        {
            _hp -= (int)val;

            if (_hp <= 0)
            {
                Destroy();
            }

            SetAnimation("shake", false, 1f);
            return val;
        }

        void Destroy()
        {
            _fix.SetActive(false);
            
            SetAnimation("break", false, 1f);
        }

        public void getKnock(Vector3 v, float p, float d) { }

        public void setFrozen(float f)
        {
            Debug.LogError("장애물은 디버프 걸리지 않음");
        }

        public void setBuff(eBuff bff, float val)
        {
            Debug.LogError("장애물은 디버프 걸리지 않음");
        }
    }
}