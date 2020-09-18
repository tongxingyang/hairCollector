using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public abstract class EnCurvedEffBase : MonoBehaviour
    {
        [SerializeField] private EnShot _type;

        protected EnShot getSkillType { get => _type; }

        protected float _dmg;
        protected float _keep;
        protected Action _del;

        protected Animator _ani;
        protected GameScene _gs;
        protected effManager _efm;
        protected Animator Ani
        {
            get
            {
                if (_ani == null)
                {
                    _ani = GetComponent<Animator>();
                }

                return _ani;
            }
        }

        public void setting(GameScene gs)
        {
            _gs = gs;
            _efm = gs.EfMng;
        }

        /// <summary> 초기화 </summary>
        public void Init(float dmg, float keep, Action del)
        {
            _dmg = dmg;
            _keep = keep;
            _del = del;
            gameObject.SetActive(true);

            prevInit();
        }

        public void onPause(bool bl)
        {
            if (Ani != null)
            {
                Ani.speed = (bl) ? 0 : 1f;
            }
        }

        public abstract void prevInit();
    }
}