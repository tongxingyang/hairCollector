using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace week
{
    public abstract class MobControl : EnemyCtrl
    {
        [Header("type")]
        [SerializeField] Mob _enemy = Mob.mob_fire;
        [SerializeField] GameObject _ice;

        protected enum stat { trace, attack, attack2, die }
        protected stat _stat;

        protected MapManager _map;

        protected float _patt = 1;
        protected float _pspeed = 5f;
        protected float _calSpeed;

        protected bool _isFrozen;
        protected float _frozenTime;
        protected float _frozenTerm;

        protected bool _isAtting;
        protected bool _is2ndMotion;
        protected float _staticCool;
        protected float _coolTime;
        protected bool _isCool;

        protected float _age;
        protected float _lifespan = 10f;
        public float PlayerDist { get; set; }

        public Mob getType { get { return _enemy; } }

        // 슈팅
        protected EnSkill_Proj _proj;
        protected float _shotTerm = 2f;
        protected Animator _ani;

        public void setting(GameScene gs)
        {
            _gs = gs;
            _player = _gs.Player;
            _efMng = _gs.EfMng;
            _enProjMng = _gs.EnProjMng;
            _map = _gs.MapMng;

            dmgFunc = _gs.DmgfntMng.getText;
            killFunc = _gs.getKill;
        }

        public void FixInit()
        {
            _ani = GetComponentInChildren<Animator>();

            _maxhp      = DataManager.GetTable<int>(DataTable.monster, _enemy.ToString(), MonsterData.hp.ToString());
            _att        = DataManager.GetTable<int>(DataTable.monster, _enemy.ToString(), MonsterData.att.ToString());
            _patt       = DataManager.GetTable<float>(DataTable.monster, _enemy.ToString(), MonsterData.patt.ToString());
            _def        = DataManager.GetTable<int>(DataTable.monster, (getType).ToString(), MonsterData.def.ToString());
            _speed      = gameValues._defaultSpeed * DataManager.GetTable<float>(DataTable.monster, _enemy.ToString(), MonsterData.speed.ToString());
            _calSpeed   = _speed;
            _pspeed     = gameValues._defaultSpeed * DataManager.GetTable<float>(DataTable.monster, _enemy.ToString(), MonsterData.pspeed.ToString());
            _exp        = DataManager.GetTable<int>(DataTable.monster, _enemy.ToString(), MonsterData.exp.ToString());
            _staticCool = DataManager.GetTable<float>(DataTable.monster, _enemy.ToString(), MonsterData.attspeed.ToString());

            otherWhenFixInit();

            RepeatInit();
        }

        public void RepeatInit()
        {
            preInit();

            _isDie = false;
            _isFrozen = false;
            _ice.gameObject.SetActive(false);
            _shotTerm = 2f;
            
            _hp = _maxhp;
            _coolTime = 0;
            _isCool = false;

            if (_spine != null)
            {
                _spine.skeleton.SetColor(Color.white);
                SetAnimation("trace", true, 1f);
            }
            _hpbar.localScale = new Vector2(_hp / _maxhp, 1f);
            _isDmgAction = false;

            //_target = target.normalized;

            applyMove();

            otherWhenRepeatInit();
        }

        #region [ move's ]

        protected abstract IEnumerator lifeCycle();
        protected abstract void switchStat(stat st);
        void applyMove()
        {
            switchStat(stat.trace);

            StartCoroutine(lifeCycle());
        }

        protected void mopTraceMove()
        {
            if (_isFrozen == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * _speedFactor * Time.deltaTime);
            }

            //checkDist();
        }

        protected void switch2ndAttMove()
        {
            if (_isFrozen == false)
            {
                if (_isAtting == false)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * _speedFactor * Time.deltaTime);
                }

                float d = PlayerDist;
                if (_isCool == false)
                {
                    if (d < 0.5f)
                    {
                        switchStat(stat.attack);
                    }
                    else if (d < 4f)
                    {
                        
                        switchStat(stat.attack2);
                    }
                }
            }

            //checkDist();
        }
        protected void switchAttckMove()
        {
            if (_isFrozen == false)
            {
                if (_isAtting == false)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * _speedFactor * Time.deltaTime);
                }

                float d = PlayerDist;
                if (_isCool == false && d < 1f)
                {
                    switchStat(stat.attack);
                }
            }
        }

        #endregion

        #region BuffEffect

        public override void setBuff(eDeBuff type, bool last, float term, float val) 
        {
            if (_bffEff == null)
            {
                _bffEff = new List<BuffEffect>();
            }

            BuffEffect dBf = new BuffEffect(type, last, term, val);

            bfApplyStat();

            _bffEff.Add(dBf);
        }

        protected virtual void setColor()
        {
            _spine.skeleton.SetColor(_originColor);    
        }

        protected void deBuffChk(float delTime)
        {
            setColor();
            _originColor = Color.white;

            if (_bffEff == null)
                return;

            for (int i = 0; i < _bffEff.Count; i++)
            {
                _bffEff[i].Term -= delTime;

                if (_bffEff[i]._eDB == eDeBuff.slow)
                {
                    if (_bffEff[i].TermOver)
                    {
                        _bffEff.RemoveAt(i);
                        bfApplyStat();
                        i--;
                    }
                }
                else if (_bffEff[i]._eDB == eDeBuff.dotDem)
                {
                    _originColor = new Color(0.8f, 0.4f, 1f);

                    if (_bffEff[i].chkOne(delTime))
                    {
                        getDamaged(_bffEff[i]._val);

                        if (_isDie)
                            break;
                    }

                    if (_bffEff[i].TermOver)
                    {
                        _bffEff.RemoveAt(i);
                        bfApplyStat();
                        i--;
                    }
                }
            }
        }

        /// <summary> 버프 스탯 적용 -> 버프/해제시  </summary>
        void bfApplyStat()
        {
            float _att = 1; 
            float _def = 1;
            float _spd = 1;
            
            for (int i = 0; i < _bffEff.Count; i++)
            {
                if (_bffEff[i]._eDB == eDeBuff.slow)
                {
                    _spd *= _bffEff[i]._val;
                }
            }

            _calSpeed = _speed * _spd;
        }

        public override void setFrozen(float term)
        {
            if (_isFrozen)
            {
                if (_frozenTerm - _frozenTime < term)
                {
                    _frozenTerm = term;
                    _frozenTime = 0;
                }
            }
            else
            {
                _isFrozen = true;
                _frozenTerm = term;
            }

            _ice.gameObject.SetActive(true);
        }

        protected void chkFrozen(float deltime)
        {
            if (_isFrozen)
            {
                _frozenTime += deltime;

                if (_frozenTime > _frozenTerm)
                {
                    _isFrozen = false;
                    _frozenTerm = 0;
                    _frozenTime = 0;
                    _ice.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region Destroy

        public override void Destroy()
        {
            preDestroy();

            _bffEff = null;

            _isDie = true;
            _isAtting = false;
            _speedFactor = 1;
        }

        public override void enemyDie()
        {
            if (_isDie == false)
            {
                // SoundManager.instance.PlaySFX(SFX.endie);
                killFunc((int)_exp);
                _efMng.makeEff(effAni.explosion, transform.position);
                otherWhenDie();
                Destroy();
            }
        }

        public void enemyDieToBoss()
        {
            if (_isDie == false)
            {
                _efMng.makeEff(effAni.explosion, transform.position);
                otherWhenDie();
                Destroy();
            }
        }

        protected void chkDestroy(float deltime)
        {
            _age += deltime;
            if (_age > _lifespan)
            {
                if (PlayerDist > _lifespan)
                {
                    _age = 0f;
                    //Debug.Log("넘 멀어!");
                    Destroy();
                }
            }            
        }

        #endregion

        #region abstract attack

        protected abstract IEnumerator mopAttack();

        #endregion

        #region collider

        public override void onPause(bool bl)
        {
            spinePause(bl);
        }

        #endregion        
    }
}