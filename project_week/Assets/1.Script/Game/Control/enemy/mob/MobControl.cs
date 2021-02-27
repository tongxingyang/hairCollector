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
        [SerializeField] Mob _enemy = Mob.fire;
        [SerializeField] protected SpriteRenderer _render;

        protected enum stat { trace, attack, attack2, die }
        protected stat _stat;

        protected MapManager _map;

        protected float MaxHp { get { return _finalStt[(int)snowStt.maxHp]; } }
        protected float Att 
        {
            get
            {
                if (_enMng.isEnemyBlind)
                {
                    int rate = UnityEngine.Random.Range(0, 100);
                    if (rate < _enMng._blindRate)
                    {
                        return 0;
                    }
                }

                return _finalStt[(int)snowStt.att] * _enMng.FieldBuff[eBuff.att] * _mobBuff[(int)eBuff.att];                
            }
        }
        protected float Def { get { return _finalStt[(int)snowStt.def] + _enMng.FieldBuff[eBuff.def] + _mobBuff[(int)eBuff.def]; } }
        protected float Speed { get { return _finalStt[(int)snowStt.speed] * _enMng.FieldBuff[eBuff.speed]; } }

        protected float[] _mobBuff; // 몹 디버프는 시간제한 없음

        protected float _patt = 1;
        protected float _pspeed = 5f;
        protected float _calSpeed;

        protected bool _isFrozen;
        protected float _frozenTime;
        protected float _frozenTerm;

        protected bool _isAtting;
        protected bool _is2ndMotion;
        protected float _coolTime;
        protected bool _isCool;

        protected float _age;
        protected float _lifespan = 10f;
        public float PlayerDist { get; set; }

        public Mob getType { get { return _enemy; } }
        float _def;

        protected Action killFunc;

        // 슈팅
        protected EnSkillControl _esc;
        protected float _shotTerm = 2f;
        protected Animator _ani;
        protected string _mobName;

        public void setting(GameScene gs)
        {
            _gs = gs;
            _player = _gs.Player;
            _enMng = _gs.EnemyMng;
            _efMng = _gs.EfMng;
            _enProjMng = _gs.EnProjMng;
            _clMng = _gs.ClockMng;
            _map = _gs.MapMng;

            dmgFunc = _gs.DmgfntMng.getText;
            killFunc = _gs.getKill;
        }

        public void FixInit(season ss)
        {
            _ani = GetComponentInChildren<Animator>();
            _standardStt = new float[(int)snowStt.max];
            _standardStt[(int)snowStt.maxHp]    = DataManager.GetTable<int>(DataTable.monster, _enemy.ToString(), MonsterData.hp.ToString());
            _standardStt[(int)snowStt.att]      = DataManager.GetTable<int>(DataTable.monster, _enemy.ToString(), MonsterData.att.ToString());
            _standardStt[(int)snowStt.def]      = DataManager.GetTable<int>(DataTable.monster, (getType).ToString(), MonsterData.def.ToString());
            _standardStt[(int)snowStt.speed]    = DataManager.GetTable<float>(DataTable.monster, _enemy.ToString(), MonsterData.speed.ToString()) * gameValues._defaultSpeed;

            _mobBuff = new float[3] { 1f, 1f, 0f };

            _patt       = DataManager.GetTable<float>(DataTable.monster, _enemy.ToString(), MonsterData.patt.ToString());            
            _calSpeed   = _standardStt[(int)snowStt.speed];
            _pspeed     = DataManager.GetTable<float>(DataTable.monster, _enemy.ToString(), MonsterData.pspeed.ToString());
            
            if (getType == Mob.fire || (int)getType % 3 == 1)
                _def = 0.5f;
            else if ((int)getType % 3 == 2)
                _def = 1f;
            else if ((int)getType % 3 == 0)
                _def = 2f;

            otherWhenFixInit();

            RepeatInit(ss);
        }

        public void RepeatInit(season ss)
        {
            preInit();

            _mobName = DataManager.GetTable<string>(DataTable.monster, getType.ToString(), ss.ToString());

            // 몹 스탯 적용
            _finalStt = new float[(int)snowStt.max];
            _finalStt[(int)snowStt.maxHp]   = _standardStt[(int)snowStt.maxHp]  * Mathf.Pow(gameValues._mobIncrease, _clMng.Day)    * _enMng.InitBff[(int)eBuff.hp];
            _hp = MaxHp;
            _finalStt[(int)snowStt.att]     = _standardStt[(int)snowStt.att]    * Mathf.Pow(gameValues._mobIncrease, _clMng.Day)    * _enMng.InitBff[(int)eBuff.att];
            _finalStt[(int)snowStt.def]     = _standardStt[(int)snowStt.def]    + (_def * _clMng.Day)                               + _enMng.InitBff[(int)eBuff.def];
            if (_finalStt[(int)snowStt.def] > 80f)
                _finalStt[(int)snowStt.def] = 80f;
            else if (_finalStt[(int)snowStt.def] < -80f)
                _finalStt[(int)snowStt.def] = -80f;

            _finalStt[(int)snowStt.speed] = _standardStt[(int)snowStt.speed];

            // Debug.Log("체 : " + MaxHp + ", 공 : " + _finalStt[(int)snowStt.att] + ", 방 : " + _finalStt[(int)snowStt.def]);

            float val = _enMng.FieldBuff[eBuff.size];
            transform.localScale = new Vector3(val, val);

            _isDie = false;
            _isFrozen = false;
            _shotTerm = 2f;           
            
            _coolTime = 0;
            _isCool = false;

            _render.color = Color.white;
            
            _isDmgAction = false;

            applyMove();

            otherWhenRepeatInit();
        }

        #region [ move's ]

        protected abstract IEnumerator lifeCycle();
        protected abstract void switchStat(stat st);
        void applyMove()
        {
            switchStat(stat.trace);
            _ani.SetTrigger($"{_mobName}_play");

            StartCoroutine(lifeCycle());
        }

        protected void mopTraceMove()
        {
            if (_isFrozen == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Speed * Time.deltaTime);
            }
        }

        protected void mopTraceLongMove()
        {
            if (_isFrozen == false)
            {
                if (PlayerDist > 2.5f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Speed * Time.deltaTime);
                }
            }
        }

        protected void switch2ndAttMove()
        {
            if (_isFrozen == false)
            {
                if (_isAtting == false)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Speed * Time.deltaTime);
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
                    transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Speed * Time.deltaTime);
                }

                float d = PlayerDist;
                if (_isCool == false && d < 1f)
                {
                    switchStat(stat.attack);
                }
            }
        }

        #endregion

        /// <summary> 받은 데미지~방어력 계산해서 </summary>
        public override float getDamaged(float val, bool ignoreDef = false)
        {
            if (ignoreDef == false) // 방어력 적용
            {
                val = val * (100f - Def) * 0.01f;
            }

            dmgFunc(transform, Convert.ToInt32(val).ToString(), dmgTxtType.standard, false);
            _hp -= val;

            if (_hp <= 0)
            {
                enemyDie();
            }

            damagedAni();
            return val;
        }

        #region BuffEffect

        protected virtual void setColor()
        {
            _render.color = _originColor;    
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

            _ani.SetTrigger("frozen");
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
                    _ani.SetTrigger($"{_mobName}_play");
                }
            }
        }

        /// <summary> 버프/디버프 </summary>
        public override void setBuff(eBuff bff, float val)
        {
            switch (bff)
            {
                case eBuff.att:
                    _mobBuff[(int)bff] *= val;
                    break;
                case eBuff.def:
                    _mobBuff[(int)bff] += val;
                    break;
                default:
                    Debug.LogError("현재 관련 디버프 없음");
                    break;
            }
        }

        /// <summary> 몹 독뎀 5% </summary>
        protected void chkDotDmg(float del)
        {
            _dot = _dotDmg.dotDmging(del);
            if (_dot > 0)
            {
                _dot = MaxHp * _dot * 0.01f;
                getDamaged(_dot, true);
            }
        }

        #endregion

        #region Destroy

        public override void Destroy()
        {
            preDestroy();

            _isDie = true;
            _isAtting = false;
        }

        public override void enemyDie()
        {            
            if (_isDie == false)
            {
                // SoundManager.instance.PlaySFX(SFX.endie);
                killFunc();
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

        #region [override]

        protected override IEnumerator damageAni()
        {
            _isDmgAction = true;
            _render.color = new Color(1, 0.4f, 0.4f);

            yield return new WaitForSeconds(0.1f);

            _render.color = _originColor;

            yield return new WaitForSeconds(0.1f);

            _render.color = new Color(1, 0.4f, 0.4f);

            yield return new WaitForSeconds(0.1f);

            _render.color = _originColor;
            _isDmgAction = false;
        }

        #endregion

        #region collider

        public override void onPause(bool bl)
        {
            _ani.speed = (bl) ? 0f : 1f;
        }

        #endregion        
    }
}