using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace week
{
    public abstract class bossControl : EnemyCtrl
    {
        [Header("type")]
        [SerializeField] Boss _boss = Boss.boss_owl;

        [SerializeField] protected GameObject _hpCase;
        [SerializeField] protected Transform _hpbar;

        protected Vector3[] _shotDir = new Vector3[4] { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
        protected enum dir { back, right, front, left }
        protected enum stat { Idle, back, Patrol, Trace, skill0, skill1, die }
        protected stat _stats;
        protected dir _dir;

        protected Action<float, float> killFunc;
        protected LandObject _home;
        protected Vector3 _homePos;
        protected readonly float _bossRange = 7.5f;
        protected readonly float _bossSkillCool = 4f;

        protected float MaxHp { get { return _finalStt[(int)enemyStt.HP]; } }
        protected float Att { get { return _finalStt[(int)enemyStt.ATT]; } }
        protected float Def { get { return _finalStt[(int)enemyStt.DEF]; } }
        protected float Speed { get { return _finalStt[(int)enemyStt.SPEED]; } }
        protected float Exp { get { return _finalStt[(int)enemyStt.EXP]; } }
        protected float Coin { get { return _finalStt[(int)enemyStt.COIN]; } }

        protected float _bossCoin = 1;

        float _skill0 = 1;
        float _skill1 = 1;
        protected float Skill0 { get; set; }
        protected float Skill1 { get; set; }

        Vector3 _direct;
        protected bool _isAppear;
        protected float _appearTime = 2f;

        public bool isFrozen { get; set; }
        public float Slow { get; set; }

        public Boss getType { get { return _boss; } }

        public override float getDamaged(attackData data)
        {
            if (data.def_Ignore == false)
            {
                data.damage = (Def >= 100f) ? 0f : data.damage * (100f - Def) * 0.01f;
            }

            dmgFunc(transform, Convert.ToInt32(data.damage).ToString(), dmgTxtType.standard, false);
            _gs.setInQuestData(inQuest_goal_key.dmg, inQuest_goal_valtype.boss, data.damage);            
            _hp -= data.damage;

            refreshHpbar();

            if (_hp <= 0)
            {
                enemyDie();
            }

            damagedAni();
            return data.damage;
        }

        public void setting(GameScene gs, Action<Transform, string, dmgTxtType, bool> dmg, Action<float, float> kill)
        {
            _player = gs.Player;
            _gs = gs;
            _efMng = gs.EfMng;
            _enProjMng = gs.EnProjMng;
            _clMng = gs.ClockMng;
            _enMng = gs.EnemyMng;

            dmgFunc = dmg;
            killFunc = kill;
        }

        public void FixInit()
        {
            _standardStt = new float[(int)enemyStt.max];
            _standardStt[(int)enemyStt.HP]      = D_boss.GetEntity(_boss.ToString()).f_hp;
            _standardStt[(int)enemyStt.ATT]     = D_boss.GetEntity(_boss.ToString()).f_att;
            _standardStt[(int)enemyStt.DEF]     = D_boss.GetEntity(_boss.ToString()).f_def;
            _standardStt[(int)enemyStt.SPEED]   = D_boss.GetEntity(_boss.ToString()).f_speed * gameValues._defaultSpeed;
            _standardStt[(int)enemyStt.EXP]     = D_boss.GetEntity(_boss.ToString()).f_exp;
            _standardStt[(int)enemyStt.COIN]    = D_boss.GetEntity(_boss.ToString()).f_coin;
            
            _skill0     = D_boss.GetEntity(_boss.ToString()).f_skill0;
            _skill1     = D_boss.GetEntity(_boss.ToString()).f_skill1;

            _dotDmg = new dotDmg();

            otherWhenFixInit();

            RepeatInit();
        }

        public void RepeatInit() 
        {
            _dotDmg.reset();

            _finalStt = new float[(int)enemyStt.max];

            _finalStt[(int)enemyStt.HP]  = _standardStt[(int)enemyStt.HP] * _enMng.MobRate * _enMng.MobDayRate;
            _finalStt[(int)enemyStt.ATT] = _standardStt[(int)enemyStt.ATT] * _enMng.MobRate * _enMng.MobDayRate;
            _finalStt[(int)enemyStt.DEF] = _standardStt[(int)enemyStt.DEF] * Mathf.Pow(1.11f, _enMng.MobDayRate);
            _finalStt[(int)enemyStt.SPEED] = _standardStt[(int)enemyStt.SPEED];
            _finalStt[(int)enemyStt.EXP] = _standardStt[(int)enemyStt.EXP];
            _finalStt[(int)enemyStt.COIN] = _standardStt[(int)enemyStt.COIN];

            Skill0 = _skill0 * _finalStt[(int)enemyStt.ATT];
            Skill1 = _skill1 * _finalStt[(int)enemyStt.ATT];

            _hp = MaxHp;
            _isDie = false;
            IsUse = true;
            _isAppear = false;
            isFrozen = false;
            Slow = 1f;

            refreshHpbar();
            _isDmgAction = false;
            gameObject.SetActive(true);

            otherWhenRepeatInit();
        }

        public override void setBuff(enemyStt bff, float val)
        {
            Debug.LogError("보스는 디버프 걸리지 않음");
        }

        protected void refreshHpbar()
        {
            _hpbar.localScale = new Vector2(_hp / MaxHp, 1f);
        }

        public void PlayObject()
        {
            StartCoroutine(lifeCycle());
        }

        public void setHome(LandObject home, Vector3 homePos)
        {
            _home = home;
            _homePos = homePos;
        }

        /// <summary> 보스 독뎀 절반 </summary>
        protected void chkDotDmg()
        {
            _dot = _dotDmg.dotDmging(deltime);
            if (_dot > 0)
            {
                Debug.Log(_dot);
                _dot = MaxHp * _dot * 0.5f * 0.01f;
                _dotData.setDot(_dot);
                _dotData.def_Ignore = true;

                getDamaged(_dotData);
            }
        }

        public override void enemyDie()
        {
            SoundManager.instance.PlaySFX(SFX.bossdie);
            killFunc(Coin, Exp);

            _player.getNamedBuff(_boss);            

            _efMng.makeEff("bossExplosion", transform.position);
            Destroy();
        }

        public void chkDestroy(LandObject land)
        {
            if (land == _home)
            {
                Destroy();
            }
        }

        protected override void Destroy()
        {
            _isDie = true;
            preDestroy();
        }

        public override void setFrozen(float term) { }

        protected virtual IEnumerator lifeCycle()
        {
            yield return null;
        }
        protected abstract void switchStat(stat st);

        public abstract void whenEnemyEnter();
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                _player.getDamaged(this, Att);
            }
            else if (collision.gameObject.tag.Equals("Enemy"))
            {
                collision.gameObject.GetComponent<MobControl>().enemyDieToBoss();
                whenEnemyEnter();
            }
        }
    }
}