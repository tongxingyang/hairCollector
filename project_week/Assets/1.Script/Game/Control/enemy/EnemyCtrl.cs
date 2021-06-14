using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace week
{
    public abstract class EnemyCtrl : poolingObject, IDamage, IPause
    {
        protected float[] _standardStt;
        protected float[] _finalStt;
        public float _hp;

        protected bool _isBoss;
        protected bool _isDie;

        protected GameScene _gs;
        protected PlayerCtrl _player;

        protected enemyManager _enMng;
        protected EnemyProjManager _enProjMng;
        protected effManager _efMng;
        protected clockManager _clMng;

        protected attackData _dotData = new attackData();
        protected dotDmg _dotDmg;
        protected float deltime = 0;
        protected float _dot = 0;
        protected Color _originColor = Color.white;

        protected Action<Transform, string, dmgTxtType, bool> dmgFunc;
        protected bool _isDmgAction;

        public virtual float getAtt { get => _finalStt[(int)enemyStt.ATT]; }
        public dotDmg DotDmg { get => _dotDmg; set => _dotDmg = value; }

        protected abstract void otherWhenFixInit();
        protected abstract void otherWhenRepeatInit(); 
        protected abstract void otherWhenDie();

        public float getHp()
        {
            return _finalStt[(int)enemyStt.HP];
        }

        public virtual float getDamaged(attackData data)
        {
            if (data.def_Ignore == false)
            {
                data.damage = data.damage * (100f - _finalStt[(int)enemyStt.DEF]) * 0.01f;
                // val = (val - _finalStt[(int)snowStt.def] > 0) ? val - _finalStt[(int)snowStt.def] : 0;
            }

            dmgFunc(transform, Convert.ToInt32(data.damage).ToString(), dmgTxtType.standard, false);
            _hp -= data.damage;

            if (_hp <= 0)
            {
                enemyDie();
            }

            damagedAni();
            return data.damage;
        }

        public void getKnock(Vector3 endP, float power = 0.05f, float duration = 0.1f)
        {
            if (_isDie == false && _isBoss == false)
            {
                // transform.DOJump(transform.position + endP, power, 1, duration);
                transform.DOMove(transform.position + endP, duration);
            }
        }

        public abstract void setFrozen(float term);
        public abstract void setBuff(enemyStt bff, float val);

        public virtual void enemyDie() { }
        protected override void Destroy() { }

        /// <summary> 외부에서 삭제 </summary>
        public void ForceDestroy()
        {
            Destroy();
        }

        #region damage Animation

        public void damagedAni()
        {
            if (_isDmgAction || gameObject.activeSelf == false)
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

        public virtual void onPause(bool bl) { }

        #endregion
    }
}