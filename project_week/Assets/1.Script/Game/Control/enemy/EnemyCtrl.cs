using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using ES3Types;

namespace week
{
    public abstract class EnemyCtrl : poolingObject, IDamage
    {
        protected float[] _standardStt;
        protected float _hp;

        protected bool _isBoss;
        protected bool _isDie;

        protected GameScene _gs;
        protected PlayerCtrl _player;

        protected enemyManager _enMng;
        protected EnemyProjManager _enProjMng;
        protected effManager _efMng;

        protected dotDmg _dotDmg;
        protected Color _originColor = Color.white;

        protected Action<int> killFunc;
        protected Action<Transform, float, dmgTxtType, bool> dmgFunc;
        protected bool _isDmgAction;

        public virtual float getDamage { get => _standardStt[(int)snowStt.att]; }
        public dotDmg DotDmg { get => _dotDmg; set => _dotDmg = value; }

        protected abstract void otherWhenFixInit();
        protected abstract void otherWhenRepeatInit(); 
        protected abstract void otherWhenDie();

        public virtual float getDamaged(float val)//, bool knockBack = false)
        {
            val = (val - _standardStt[(int)snowStt.def] > 0) ? val - _standardStt[(int)snowStt.def] : 0;

            dmgFunc(transform, (int)val, dmgTxtType.standard, false);
            _hp -= val;

            if (_hp <= 0)
            {
                enemyDie();
            }

            damagedAni();
            return val;
        }

        public void getKnock(Vector3 endP, float power = 0.05f, float duration = 0.1f)
        {
            if (_isDie == false && _isBoss == false)
            {
                transform.DOJump(transform.position + endP, power, 1, duration);
            }
        }

        public virtual void enemyDie() { }
        public abstract void setFrozen(float term);
        public override void Destroy() { }

        #region damage Animation

        public void damagedAni()
        {
            if (_isDmgAction && gameObject.activeSelf == false)
            {
                return;
            }

            if (_hp > 0)
            {
                try
                {
                    StartCoroutine(damageAni());
                }
                catch
                {
                    Debug.LogError("setactive : " + gameObject.activeSelf);
                }
            }
        }
        protected abstract IEnumerator damageAni();

        #endregion
    }
}