using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace week
{
    public abstract class EnemyCtrl : poolingObject, IDamage
    {
        [SerializeField] protected GameObject _hpCase;
        [SerializeField] protected Transform _hpbar;
        
        protected float _hp = 20;
        protected float _maxhp = 20;
        protected float _att = 1;
        protected float _def = 0;
        protected float _speed = gameValues._defaultSpeed;

        protected float _speedFactor = 1f;
        protected float _exp = 2;

        protected bool _isBoss;
        protected bool _isDie;

        protected GameScene _gs;
        protected PlayerCtrl _player;

        protected EnemyProjManager _enProjMng;
        protected effManager _efMng;

        protected List<BuffEffect> _bffEff;
        protected Color _originColor = Color.white;

        protected Action<int> killFunc;
        protected Action<Transform, int, dmgTxtType, bool> dmgFunc;
        protected bool _isDmgAction;

        public float getDamage { get => _att; }

        protected abstract void otherWhenFixInit();
        protected abstract void otherWhenRepeatInit(); 
        protected abstract void otherWhenDie();

        public bool getDamaged(float val)//, bool knockBack = false)
        {
            val = (val - _def > 0) ? val - _def : 0;

            dmgFunc(transform, (int)val, dmgTxtType.standard, false);
            _hp -= val;
            _hpbar.localScale = new Vector2(_hp / _maxhp, 1f);

            if (_hp <= 0)
            {
                enemyDie();
                return true;
            }

            damagedAni();
            return false;
        }

        public virtual void setBuff(eDeBuff type, bool last, float term, float val) { }

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

        protected virtual IEnumerator damageAni()
        {
            if (_spine == null)
            {
                yield break;
            }
            _isDmgAction = true;
            _spine.skeleton.SetColor(new Color(1, 0.4f, 0.4f));

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(_originColor);

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(new Color(1, 0.4f, 0.4f));

            yield return new WaitForSeconds(0.1f);

            _spine.skeleton.SetColor(_originColor);
            _isDmgAction = false;
        }

        #endregion
    }
}