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

        protected float MaxHp { get { return _finalStt[(int)enemyStt.HP]; } }
        protected virtual float Att { get { return _finalStt[(int)enemyStt.ATT]     * _enMng.FieldBuff[enemyStt.ATT] * _mobBuff[(int)enemyStt.ATT]; } }
        protected float Def { get { return _finalStt[(int)enemyStt.DEF]     + _enMng.FieldBuff[enemyStt.DEF] + _mobBuff[(int)enemyStt.DEF]; } }
        protected float Speed { get { return _finalStt[(int)enemyStt.SPEED] * _enMng.FieldBuff[enemyStt.SPEED] * _mobBuff[(int)enemyStt.SPEED] * _increaseSpeed; } }
        protected float _increaseSpeed = 1f;

        protected float[] _mobBuff; // 몹 디버프는 시간제한 없음

        protected float _patt = 1;
        protected float _pspeed = 5f;

        public bool _isFrozen;
        protected float _frozenTime;
        protected float _frozenTerm;

        protected bool _isAtting;
        protected bool _is2ndMotion;
        protected float _coolTime;
        protected bool _isCool;

        protected SkillKeyList _lastAttack;

        protected float _age;
        protected float _lifespan = 8.5f;
        public float PlayerDist { get; set; }

        protected Action<float, float, Mob> killFunc;
        float _exp;
        float _coin;

        // 슈팅
        protected EnSkillControl _esc;
        protected float _shotTerm = 2f;

        protected season _season;
        protected Animator _ani;
        protected string _aniKey;

        public Mob EnemyType { get => _enemy; }

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
            killFunc = _gs.kill_mob;
            _exp = D_monster.GetEntity(_enemy.ToString()).f_exp;
            _coin = D_monster.GetEntity(_enemy.ToString()).f_coin;
        }

        public void FixInit(season ss)
        {
            _ani = GetComponentInChildren<Animator>();
            _standardStt = new float[(int)enemyStt.max];
            _standardStt[(int)enemyStt.HP]      = D_monster.GetEntity(_enemy.ToString()).f_hp;
            _standardStt[(int)enemyStt.ATT]     = D_monster.GetEntity(_enemy.ToString()).f_att;
            _standardStt[(int)enemyStt.DEF]     = D_monster.GetEntity(_enemy.ToString()).f_def;
            _standardStt[(int)enemyStt.SPEED]   = D_monster.GetEntity(_enemy.ToString()).f_speed * gameValues._defaultSpeed;
            _standardStt[(int)enemyStt.EXP]     = D_monster.GetEntity(_enemy.ToString()).f_exp;
            _standardStt[(int)enemyStt.COIN]    = D_monster.GetEntity(_enemy.ToString()).f_coin;

            _mobBuff = new float[(int)enemyStt.max] { 1f, 1f, 0f, 1f, 1f, 1f, 1f };

            _patt       = D_monster.GetEntity(_enemy.ToString()).f_patt;            
            _pspeed     = D_monster.GetEntity(_enemy.ToString()).f_pspeed;

            otherWhenFixInit();

            RepeatInit(ss);
        }

        public void RepeatInit(season ss)
        {
            preInit();
            _season = ss;

            _aniKey = D_season.GetEntity(ss.ToString()).Get<string>(EnemyType.ToString());
            if (_aniKey.Equals("all"))
            {
                ss = (season)UnityEngine.Random.Range(0, 4);
                _aniKey = D_season.GetEntity(ss.ToString()).Get<string>(EnemyType.ToString());
            }
            _aniKey += "_play";

            // 몹 스탯 적용
            _finalStt = new float[(int)enemyStt.max];
            _finalStt[(int)enemyStt.HP]   = _standardStt[(int)enemyStt.HP] * _enMng.InitBff[(int)enemyStt.HP] * _enMng.MobRate * _enMng.MobDayRate;
            _hp = MaxHp;
            _finalStt[(int)enemyStt.ATT] = _standardStt[(int)enemyStt.ATT] * _enMng.InitBff[(int)enemyStt.ATT] * _enMng.MobRate * _enMng.MobDayRate;
            _finalStt[(int)enemyStt.DEF] = _standardStt[(int)enemyStt.DEF] * _enMng.MobDefRate * D_level.GetEntity(_gs.StageLevel.ToString()).f_defrate + _enMng.InitBff[(int)enemyStt.DEF];
            _finalStt[(int)enemyStt.SPEED] = _standardStt[(int)enemyStt.SPEED];

            _increaseSpeed = _enMng.MobIncSpeed;

            float val = _enMng.FieldBuff[enemyStt.SIZE];
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
            _ani.SetTrigger(_aniKey);

            StartCoroutine(lifeCycle());
        }

        protected void mopTraceMove()
        {
            if (_isFrozen == false)
            {
                //Debug.Log(Speed+"("+_outOfViewSpeed+")");
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

        protected virtual void check_mobStair(float deltime)
        {
            chkDotDmg(deltime);
            chkDestroy(deltime);
            chkFrozen(deltime);            
        }

        /// <summary> 받은 데미지~방어력 계산해서 </summary>
        public override float getDamaged(attackData data)
        {
            //Debug.Log(data.type + " : " + data.damage);
            if (data.def_Ignore == false) // 방어력 적용
            {
                data.damage = data.damage * (100f - Def) * 0.01f;
            }

            dmgFunc(transform, Convert.ToInt32(data.damage).ToString(), dmgTxtType.standard, false);

            _lastAttack = data.type;
            _gs.setInQuestData(inQuest_goal_key.dmg, inQuest_goal_valtype.mob, data.damage);
            _hp -= data.damage;

            if (_hp <= 0)
            {
                if (data.type == SkillKeyList.SnowBall)
                {
                    _gs.setInQuestData(inQuest_goal_key.skill, inQuest_goal_valtype.cut, 1);
                }

                enemyDie();
            }

            damagedAni();
            return data.damage;
        }

        #region BuffEffect

        protected virtual void setColor()
        {
            _render.color = _originColor;    
        }

        public override void setFrozen(float term)
        {
            if (IsUse == false)
                return;

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
                SoundManager.instance.PlaySFX(SFX.icing);

                _isFrozen = true;
                _frozenTerm = term;
                _ani.SetTrigger("frozen");
            }

        }

        protected void chkFrozen(float deltime)
        {
            if (_isFrozen)
            {
                _frozenTime += deltime;

                if (_frozenTime >= _frozenTerm)
                {
                    _isFrozen = false;
                    _frozenTerm = 0;
                    _frozenTime = 0;
                    _ani.SetTrigger(_aniKey);                    
                }
            }
        }

        /// <summary> 버프/디버프 </summary>
        public override void setBuff(enemyStt bff, float val)
        {
            switch (bff)
            {
                case enemyStt.ATT:
                    _mobBuff[(int)bff] *= val;
                    break;
                case enemyStt.DEF:
                    _mobBuff[(int)bff] += val;
                    break;
                case enemyStt.SPEED:
                    _mobBuff[(int)bff] *= val;
                    break;
                default:
                    Debug.LogError("현재 관련 디버프 없음");
                    break;
            }
        }

        /// <summary> 몹 독뎀 </summary>
        protected virtual void chkDotDmg(float del)
        {
            _dot = _dotDmg.dotDmging(del);
            if (_dot > 0)
            {
                _dot = MaxHp * _dot * 0.01f;
                _dotData.setDot(_dot);

                getDamaged(_dotData);
            }
        }

        #endregion

        #region Destroy

        protected override void Destroy()
        {
            _enMng.getOffEnemyList(this);

            preDestroy();

            _isDie = true;
            _isAtting = false;
        }

        public override void enemyDie()
        {            
            if (_isDie == false)
            {
                // SoundManager.instance.PlaySFX(SFX.endie);
                killFunc(_exp, _coin, _enemy);
                _efMng.makeEff("explosion", transform.position);
                otherWhenDie(); 

                Destroy();
            }
        }

        public void enemyDieToBoss()
        {
            if (_isDie == false)
            {
                _efMng.makeEff("explosion", transform.position);
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